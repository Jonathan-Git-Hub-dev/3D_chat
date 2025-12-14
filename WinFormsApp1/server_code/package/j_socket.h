
#define MAX_CLIENTS 5
#define TCP_PORT 8080
#define UDP_PORTS 8081, 8082, 8083, 8084, 8085
#define CLIENTS false, false, false, false, false

#include <netinet/in.h>
#include <arpa/inet.h>


int Initialize_Socket();


void Configure_Socket(int *server_fd);

void Bind_Socket(int *server_fd,  struct sockaddr_in *address);

void Listen_Socket(int *server_fd);

void UDP_Socket(int *sockfd);

void Set_Sockaddr_In(struct sockaddr_in *servaddr , int port);

