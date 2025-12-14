#include <stdio.h>
#include <pthread.h>
#include <stdlib.h>
#include <string.h>
#include <netinet/in.h>
#include <sys/socket.h>
#include <unistd.h>
#include <stdbool.h>
#include <pthread.h>
#include <arpa/inet.h>
#include <poll.h>
#include <limits.h>

#include "j_thread.h"
#include "j_socket.h"
#include "j_user_info.h"
#include "j_temp.h"
#include "j_globals.h"

#include <sys/socket.h>

#define MAXLINE 10000

extern struct user_data users[MAX_USERS];
extern char **audio_samples;
extern pthread_mutex_t audio_mutex[MAX_USERS];
extern char final_audio[FULL_SAMPLE_LEN];
extern pthread_mutex_t main_audio_mutex;
extern int *last_audio_samples;

void exact_copy_n(char *dest, char *src, int n)
{
	for(int i=0; i<n; i++)
	{
		dest[i] = src[i];
	}
}

void* handle_connection(void* ptr)
{
	int our_socket = ((int*)ptr)[0];
	int client_index = ((int*)ptr)[1];
	int udp = ((int*)ptr)[2];

	free(ptr);



	int sockfd;
    char in_buffer[10000];
	char out_buffer[10000];
    
    struct sockaddr_in servaddr, cliaddr, unused_address;
	memset(&servaddr, 0, sizeof(servaddr));
    memset(&cliaddr, 0, sizeof(cliaddr));
	memset(&unused_address, 0, sizeof(unused_address));

    // Create a UDP socket
    UDP_Socket(&sockfd);
	Set_Sockaddr_In(&servaddr, udp);
	Bind_Socket(&sockfd, &servaddr);


    socklen_t len = sizeof(cliaddr);
	socklen_t l2 = sizeof(unused_address);
    int n;

	//get udp port to send to
	//printf("%d first \n", client_index);
	n = recvfrom(sockfd, (char *)in_buffer, MAXLINE, MSG_WAITALL, (struct sockaddr *)&cliaddr, &len);
	//printf("%d after first \n", client_index);
	if (n > 0)
    {
     	char ip_str[INET_ADDRSTRLEN];
        inet_ntop(AF_INET, &(cliaddr.sin_addr), ip_str, sizeof(ip_str));
    }

	//polling
	struct pollfd fds[1]; // Array to hold pollfd structures for monitored file descriptors
    fds[0].fd = sockfd;
    fds[0].events = POLLIN; // Monitor for incoming data (readability)
    fds[0].revents = 0; // Will be set by poll() to indicate events that occurred


	struct timespec ts;
	ts.tv_sec = -1;
	ts.tv_nsec = -1;
	struct timespec new_time;
	struct timespec ts2;
	ts2.tv_sec = -1;
        ts2.tv_nsec = -1;


	//read data when we have a packet
	//send data when x time has passed (do later)
    	while (true) { // Loop to continuously receive and respon
		//printf("Pre rec\n");
		int timeout_ms = 10; // Timeout in milliseconds (e.g., 1 second)
    		int ready_fds = poll(fds, 1, timeout_ms);

        	if(ready_fds > 0)//has data
		{
			n = recvfrom(sockfd, (char *)in_buffer, MAXLINE, MSG_WAITALL, (struct sockaddr *)&unused_address, &l2); //buffer[n] = '\0';

			if (n > 0)
                	{
                        	char ip_str[INET_ADDRSTRLEN];
                        	inet_ntop(AF_INET, &(unused_address.sin_addr), ip_str, sizeof(ip_str));
                        	//printf("Source IP: %s, Port: %d\n", ip_str, ntohs(unused_address.sin_port));

				in_buffer[n] = '\0';

				if(in_buffer[n-1] == 'A')
				{
                                        //sendto(sockfd, (const char *)in_buffer, n, MSG_CONFIRM, (const struct sockaddr *)&cliaddr, len);
					//printf("locking mutex %d\n", client_index);
					pthread_mutex_lock(&(audio_mutex[client_index]));

					exact_copy_n(audio_samples[client_index], in_buffer, SAMPLE_LEN);
					last_audio_samples[client_index]++;

					pthread_mutex_unlock(&(audio_mutex[client_index]));
				}
				else//'S' spatial data
				{
					in_buffer[n-1] = '\0';
					//printf("spacial update\n");
					decode_data(in_buffer, client_index);
				}
                	}

		}


		//send spatial data occsaionally
		clock_gettime(CLOCK_MONOTONIC, &new_time);
                if((new_time.tv_sec - ts.tv_sec) *1000000000 + new_time.tv_nsec - ts.tv_nsec >= SPACIAL_TIME)//off dome
                {
			encode_data(out_buffer);

                        //sendto(sockfd, (const char *)out_buffer, strlen(out_buffer), MSG_CONFIRM, (const struct sockaddr *)&cliaddr, len);

                        //save for later
                        ts.tv_sec = new_time.tv_sec;
                        ts.tv_nsec = new_time.tv_nsec;

               		//printf("Reply sent to client (%s).\n", "dw");
                }

		//send audio data occasionally
		clock_gettime(CLOCK_MONOTONIC, &new_time);
                if((new_time.tv_sec - ts2.tv_sec) *1000000000 + new_time.tv_nsec - ts2.tv_nsec >= AUDIO_TIME)//off dome
                {
                        //encode_data(out_buffer);
			pthread_mutex_lock(&main_audio_mutex);



                        if(sendto(sockfd, (const char *)final_audio, SAMPLE_LEN+1, MSG_CONFIRM, (const struct sockaddr *)&cliaddr, len) < 0) {
			//if(sendto(sockfd, (const char *)audio_samples[0], SAMPLE_LEN+1, MSG_CONFIRM, (const struct sockaddr *)&cliaddr, len) < 0) {
			//if(sendto(sockfd, (const char *)"hello", 5, MSG_CONFIRM, (const struct sockaddr *)&cliaddr, len ) < 0){
				perror("sendto failed"); // Call perror if sendto() fails
        			exit(EXIT_FAILURE);
    			}
			//sendto(sockfd, (const char *)out_buffer, strlen(out_buffer), MSG_CONFIRM, (const struct sockaddr *)&cliaddr, len);


			pthread_mutex_unlock(&main_audio_mutex);



                        //save for later
                        ts2.tv_sec = new_time.tv_sec;
                        ts2.tv_nsec = new_time.tv_nsec;

                        //printf("Audio Reply sent to client (%s).\n", "dw");
                }

    }

    close(sockfd); // Close the socket (unreachable in this infinite loop)
    return 0;
}
