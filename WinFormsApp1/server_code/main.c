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

//still under construction

struct user_data users[MAX_USERS];

void* simulate(void*);


int main(void)
{
	int my_array[] = {UDP_PORTS};
	bool clients[] = {CLIENTS};

    int server_fd, new_socket;
    ssize_t valread;
	struct sockaddr_in address;
    socklen_t addrlen = sizeof(address);
    char buffer[1024] = { 0 };


	//create array of users
	for(int i=0; i< MAX_USERS; i++)
	{
		create_user_data( &(users[i]) );
	}

	//temporay second user (simultion) //////////////////////////////////////////////////////////
	struct position p;
        p.x = -1;
        p.y = -3;
        p.z = 1;
    struct colour c;
        c.red = 200;
        c.green = 200;
        c.blue = 10;
        
    update_user_data( &(users[3]), p,c, 1, 5000);
	
	//create a thread to update user(4) to simulate active user
	pthread_t sim_thread;
	pthread_create(&sim_thread, NULL, simulate, NULL);
	
	////////////////////////////////////////////////////////////////////////////////////////////


	



    address.sin_family = AF_INET;
    address.sin_addr.s_addr = INADDR_ANY;
    address.sin_port = htons(TCP_PORT);

    server_fd = Initialize_Socket();
	Configure_Socket(&server_fd);
	Bind_Socket(&server_fd, &address);
	Listen_Socket(&server_fd);



	//this needs serious work
	while(true)
	{
		//check if we have a free client slot
		//mutex
		int index = -1;
		//continue;
		
		//we can serve a client so listen out for one
		//handle this error better
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
                                //drop mutex
                        }
                }

                if(index == -1)
                {
                        //continue;
                	//kill this cnnection
			if (shutdown(new_socket, SHUT_WR) == -1) {
        			perror("shutdown failed");
        			// Handle error
    			}
			continue;
		}

		//send initail data
		char str[100];
        	//send user the udp port to use
        	sprintf(str, "%d,%d", index, my_array[index]);
        	send(new_socket, str, strlen(str), 0);
        //UDP_PORTS

		//char *client_ip_address = inet_ntoa(address.sin_addr);
    		//printf("Client connected from IP: %s\n", client_ip_address);
		//call thread to handle this client
		pthread_t thread;
		int *connection = malloc(3 * sizeof(int));
		connection[0] = new_socket;
		connection[1] = index;
		connection[2] = my_array[index];
		pthread_create(&thread, NULL, handle_connection, connection);
		
		// closing the connected socket
        	//close(new_socket);

	}

	//this only get call if this process is killed?
    	close(server_fd);
    	return 0;
}


void* simulate(void*)
{
	//this should be mutexed

	while(true)
	{
		int temp = 100 + users[3].angle;

		if(temp >= 35999)
		{
			temp = 0;
		}

		users[3].angle = temp;
		sleep(1);
	}
}



