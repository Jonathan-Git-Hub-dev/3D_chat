#include <netinet/in.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <sys/socket.h>
#include <unistd.h>
#include <stdbool.h>
#include <pthread.h>

#include "j_socket.h"


int Initialize_Socket()
{
	int server_fd;
	if ((server_fd = socket(AF_INET, SOCK_STREAM, 0)) < 0)
	{
        	perror("socket failed");
        	exit(EXIT_FAILURE);
    	}
	return server_fd;
}

void Configure_Socket(int *server_fd)
{
	int opt = 1;

	/*if (setsockopt(*server_fd, SOL_SOCKET,SO_REUSEADDR | SO_REUSEPORT, &opt, sizeof(opt)))
	{
        	perror("setsockopt");
        	exit(EXIT_FAILURE);
    	}*/
	if (setsockopt(*server_fd, SOL_SOCKET, SO_KEEPALIVE, &opt, sizeof(opt)) < 0)
	{
    		perror("setsockopt(SO_KEEPALIVE) failed");
		exit(EXIT_FAILURE);
	}
}

void Bind_Socket(int *server_fd, struct sockaddr_in *address)
{
    	// Forcefully attaching socket to the port 8080
    	if (bind(*server_fd, (struct sockaddr*)address, sizeof(*address)) < 0) {
        	perror("bind failed");
        	exit(EXIT_FAILURE);
    	}
}

void Listen_Socket(int *server_fd)
{
	if (listen(*server_fd, MAX_CLIENTS) < 0)
	{
        	perror("listen");
        	exit(EXIT_FAILURE);
    	}
}



