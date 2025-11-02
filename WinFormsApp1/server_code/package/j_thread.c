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

#include "j_thread.h"
#include "j_socket.h"
#include "j_user_info.h"

#include <sys/socket.h>

#define MAXLINE 10000

extern struct user_data users[MAX_USERS];

void encode_data(char* buffer)
{
	buffer[0] = '\0';
	//split useres with $
	//split data points with spaces
	//split multiple  data feilds wiht ,
	for(int i=0; i<MAX_USERS; i++)
	{
		//strcat(buffer, );
		char single_buffer[200] = "";

		sprintf(single_buffer, "%lf,%lf,%lf %d %d,%d,%d %d %d",
			users[i].user_position.x,
			users[i].user_position.y,
			users[i].user_position.z,

			users[i].angle,

			users[i].user_colour.red,
			users[i].user_colour.green,
			users[i].user_colour.blue,

			users[i].asset,

			(users[i].online ? 1 : 0)
		);

		if(i != MAX_USERS-1)
		{
			strcat(single_buffer, "$");
		}

		strcat(buffer, single_buffer);

	}
}

void decode_data(char* str, int index)
{
	//user sends data split by spaces
	//0,0,0 0 255,0,0 0
	//postion angel colour asset
	//x,y,z angle red,green,blue asset
	const char* main_delimiter = " ";
	const char* secondary_delimiter = ",";
	char* position_pointer = strtok(str, main_delimiter);
	char* angle_pointer = strtok(NULL, main_delimiter);
	char* colour_pointer = strtok(NULL, main_delimiter);
	char* asset_pointer = strtok(NULL, main_delimiter);

	//printf("positon: %s\n", position_pointer);//printf("angle: %s\n", angle_pointer);//printf("colour %s\n", colour_pointer);//printf("asset: %s\n", asset_pointer);

	char* pos_x = strtok(position_pointer, secondary_delimiter);
	char* pos_y = strtok(NULL, secondary_delimiter);
	char* pos_z = strtok(NULL, secondary_delimiter);

	char* temp_red = strtok(colour_pointer, secondary_delimiter);
	char* temp_green = strtok(NULL, secondary_delimiter);
	char* temp_blue = strtok(NULL, secondary_delimiter);

	double x = atof(pos_x);
	double y = atof(pos_y);
	double z = atof(pos_z);
	struct position p;
	p.x = x;
	p.y = y;
	p.z = z;

	int angle = atoi(angle_pointer);

	int red = atoi(temp_red);
	int green = atoi(temp_green);
	int blue = atoi(temp_blue);
	struct colour c;
	c.red = red;
	c.green = green;
	c.blue = blue;

	int asset = atoi(asset_pointer);

	update_user_data( &(users[index]), p,c, asset, angle);
}



void* handle_connection(void* ptr)
{
	int our_socket = ((int*)ptr)[0];
	int client_index = ((int*)ptr)[1];
	int udp = ((int*)ptr)[2];

	free(ptr);

	printf("our_socket: %d, client_index: %d, udp: %d\n", our_socket, client_index, udp);


	int sockfd;
    char buffer[10000];
    char *hello = "Hello from UDP server";
    struct sockaddr_in servaddr, cliaddr, unused_address;

    // Create a UDP socket
    if ((sockfd = socket(AF_INET, SOCK_DGRAM, 0)) < 0) {
        perror("socket creation failed");
        exit(EXIT_FAILURE);
    }

    memset(&servaddr, 0, sizeof(servaddr));
    memset(&cliaddr, 0, sizeof(cliaddr));
	memset(&unused_address, 0, sizeof(unused_address));

    // Server information
    servaddr.sin_family = AF_INET; // IPv4
    servaddr.sin_addr.s_addr = INADDR_ANY; // Listen on all available interfaces
    servaddr.sin_port = htons(udp); // Port to listen on

    // Bind the socket to the server address
    if (bind(sockfd, (const struct sockaddr *)&servaddr, sizeof(servaddr)) < 0) {
        perror("bind failed");
        exit(EXIT_FAILURE);
    }

    	socklen_t len;
	socklen_t l2;
    int n;

    len = sizeof(cliaddr); // Store the size of the client address structure
	l2 = sizeof(unused_address);

	//get udp port to send to
	n = recvfrom(sockfd, (char *)buffer, MAXLINE, MSG_WAITALL, (struct sockaddr *)&cliaddr, &len);
    //printf("UDP Server listening on port %d...\n", udp);

    while (true) { // Loop to continuously receive and respon
        n = recvfrom(sockfd, (char *)buffer, MAXLINE, MSG_WAITALL, (struct sockaddr *)&unused_address, &l2); //buffer[n] = '\0';
	decode_data(buffer, client_index);
        //printf("Client message: %s\n", buffer);

	encode_data(buffer);
        if(sendto(sockfd, (const char *)buffer, strlen(buffer), MSG_CONFIRM, (const struct sockaddr *)&cliaddr, len) < 0)
	{
		printf("failed to send\n");
	}
        printf("Reply sent to client.\n");
    }

    close(sockfd); // Close the socket (unreachable in this infinite loop)
    return 0;
}
