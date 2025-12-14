#include "j_thread.h"
#include "j_socket.h"
#include "j_user_info.h"

#include <stdbool.h>
#include <string.h>
#include <stdio.h>
#include <stdlib.h>

extern struct user_data users[MAX_USERS];

void encode_data(char* buffer)
{
        buffer[0] = '\0';
        //split useres with $
        //split data points with spaces
        //split multiple  data feilds wiht ,
        //printf("1\n");
        for(int i=0; i<MAX_USERS; i++)
        {
                        //printf("7\n");
                //strcat(buffer, );
                char single_buffer[2000] = "";

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
                        //printf("8\n");

                if(i != MAX_USERS-1)
                {
                        strcat(single_buffer, "$");
                }

                strcat(buffer, single_buffer);
                        //printf("9\n");
        }
	strcat(buffer, "S");
        //printf("got ddata from an end user ((%d\n", i);
}

void decode_data(char* str, int index)
{
	printf("recieved (%s)\n", str);
        if(strcmp(str, "sending useles data for binding") == 0)
        {
                return;
        }

	//char hold[50000] = "";
	//strcpy(str, hold);
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
	char* counter_tmp = strtok(NULL, main_delimiter);
	int ctr = atoi(counter_tmp);
          //      printf("2\n");
        //printf("positon: %s\n", position_pointer);//printf("angle: %s\n", angle_pointer);//printf("colour %s\n", colour_pointer);//printf("asset: %s\n", asset_pointer);

        char* pos_x = strtok(position_pointer, secondary_delimiter);
        char* pos_y = strtok(NULL, secondary_delimiter);
        char* pos_z = strtok(NULL, secondary_delimiter);

        char* temp_red = strtok(colour_pointer, secondary_delimiter);
        char* temp_green = strtok(NULL, secondary_delimiter);
        char* temp_blue = strtok(NULL, secondary_delimiter);
        //printf("3\n");
        //printf("data(%s)(%s)(%s)\n", pos_x, pos_y, pos_z);
        double x = atof(pos_x);
        double y = atof(pos_y);
        double z = atof(pos_z);
        struct position p;
        //printf("3.1\n");
        p.x = x;
        p.y = y;
        p.z = z;
        //printf("4\n");
        int angle = atoi(angle_pointer);

        int red = atoi(temp_red);
        int green = atoi(temp_green);
        int blue = atoi(temp_blue);
        struct colour c;
        c.red = red;
        c.green = green;
        c.blue = blue;
        //        printf("5\n");
        int asset = atoi(asset_pointer);
	/*if(asset > 3)
	{
		printf("we have recieved a bad asset value (%d) recieved by user %d\n", asset, index);
		//printf("%s\n", hold);
	}*/


	printf("we recieved (%d) from (%d) in transaction (%d)\n", asset, index, ctr);
        update_user_data( &(users[index]), p,c, asset, angle);
}
