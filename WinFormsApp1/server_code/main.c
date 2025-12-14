#include <netinet/in.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <sys/socket.h>
#include <unistd.h>
#include <stdbool.h>
#include <pthread.h>
#include <arpa/inet.h>
#include <sys/socket.h>
#include <netinet/in.h>

#include "package/j_socket.h"
#include "package/j_thread.h"
#include "package/j_user_info.h"
#include "package/j_audio.h"

//#define MAX_USERS 4

struct user_data users[MAX_USERS];
 
void* simulate(void*);


int main(void)
{
	//audio
	initiate_audio_samples();
	initiate_audio_mutexs();

	int my_array[] = {UDP_PORTS};
	bool clients[] = {CLIENTS};

    int server_fd, new_socket;
    ssize_t valread;
	struct sockaddr_in address;
    socklen_t addrlen = sizeof(address);
    char buffer[1024] = { 0 };


	//create array of users
	//struct user_data users[MAX_USERS];
	for(int i=0; i< MAX_USERS; i++)
	{
		create_user_data( &(users[i]) );
	}
	//temporay second user
	struct position p;

	//create a thread to update user(4) to simulate active user
	pthread_t sim_thread;
	pthread_create(&sim_thread, NULL, mix_audio, NULL);



	Set_Sockaddr_In(&address, TCP_PORT);
    server_fd = Initialize_Socket();
	Configure_Socket(&server_fd);
	Bind_Socket(&server_fd, &address);
	Listen_Socket(&server_fd);

	while(true)//ctrl cancel
	{
		int index = -1;
		
        if ((new_socket = accept(server_fd, (struct sockaddr*)&address, &addrlen)) < 0)
        {
            perror("accept");
            exit(EXIT_FAILURE);
        }

		//now check for spots
		for(int i=0; i < MAX_CLIENTS; i++)
        {
            if(!clients[i])
            {
                index = i;
                break;
                clients[i] = true;
            }
        }

        if(index == -1)
		{//no open thread so kill
			if (shutdown(new_socket, SHUT_WR) == -1)
			{
        		perror("shutdown failed");
				exit(-1);
    		}
			continue;
		}

		//send initail data
		char str[100];
        sprintf(str, "%d,%d", index, my_array[index]);
        send(new_socket, str, strlen(str), 0);
        

		//thread for handling connection
		pthread_t thread;
		int *connection = malloc(3 * sizeof(int));
		connection[0] = new_socket;
		connection[1] = index;
		connection[2] = my_array[index];
		printf("our new connections port %d\n", my_array[index] );
		pthread_create(&thread, NULL, handle_connection, connection);
		clients[index] = true;
	}
}
