#include <netinet/in.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <sys/socket.h>
#include <unistd.h>
#include <stdbool.h>
#include <pthread.h>
#include <stdbool.h>

#include "j_user_info.h"

void print_user_data(struct user_data *instance)
{
	printf("user is %s\n", (instance->online ? "online" : "offline"));
	printf("user is positioned at (%lf,%lf,%lf)\n", instance->user_position.x, instance->user_position.y, instance->user_position.z);
	printf("the colour of this user (%d,%d,%d)\n", instance->user_colour.red, instance->user_colour.green, instance->user_colour.blue);
	printf("user angle %d, user asset %d\n", instance->angle, instance->asset);
}

void create_user_data(struct user_data* instance)
{
	instance->user_position.x = -1;
	instance->user_position.y = -1;
	instance->user_position.z = -1;

	instance->user_colour.red = -1;
	instance->user_colour.green = -1;
	instance->user_colour.blue = -1;


	instance->asset = -1;
	instance->angle = -1;

	instance->online = false;
}

void update_user_data(struct user_data* instance, struct position p, struct colour c, int asset, int angle)
{
	//printf("in update updataing index");

	instance->user_position.x = p.x;
        instance->user_position.y = p.y;
        instance->user_position.z = p.z;

        instance->user_colour.red = c.red;
        instance->user_colour.green = c.green;
        instance->user_colour.blue = c.blue;


        instance->asset = asset;
        instance->angle = angle;

        instance->online = true;
}

void clear_user_data(struct user_data* instance)
{
	printf("\n\ncleare user data to be implemented later\n\n");
}
