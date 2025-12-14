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


char **audio_samples;
int *last_audio_samples;

int internal_last_audio_samples[MAX_USERS];

pthread_mutex_t audio_mutex[MAX_USERS];
pthread_mutex_t main_audio_mutex;

char final_audio[FULL_SAMPLE_LEN];

void initiate_audio_mutexs()
{
	for (int i = 0; i < MAX_USERS; i++)
	{
        if(pthread_mutex_init(&audio_mutex[i], NULL) < 0)
		{
			perror("failed to initiate mutex");
			exit(-1);
		}
    }

	pthread_mutex_init(&main_audio_mutex, NULL);
}

void initiate_audio_samples()
{
	audio_samples = malloc(sizeof(char*) *  MAX_USERS);
	if(audio_samples == NULL)
	{
		perror("malloc failed");
		exit(-1);
	}

	last_audio_samples = malloc(sizeof(int) * MAX_USERS);
	if(last_audio_samples == NULL)
	{
		perror("malloc failed");
		exit(-1);
	}

	for(int i = 0; i< MAX_USERS; i++)
	{
		audio_samples[i] = malloc( FULL_SAMPLE_LEN );
		if(audio_samples[i] == NULL)
		{
			perror("malloc failed");
			exit(-1);
		}
		
		audio_samples[i][SAMPLE_LEN] = 'A';
    	audio_samples[i][SAMPLE_LEN + 1] = '\0';
		
		last_audio_samples[i] = -1;
		internal_last_audio_samples[i] = -1;

	}

	final_audio[SAMPLE_LEN] = 'A';
    final_audio[SAMPLE_LEN + 1] = '\0';
}


int32_t sample_to_int(char *ptr)
{
	int32_t *int_ptr = (int32_t *)ptr;
    return *int_ptr;
}

void mix_audio_func()
{
	//short audio_sample;

	for(int j = 0; j < SAMPLE_LEN; j+=4)
	{//for samples
		int mixed_audio_sample = 0;

		for(int i = 0; i < MAX_USERS; i++)
		{
			if(last_audio_samples[i] == internal_last_audio_samples[i])
			{//if no new audio sample
				continue;
			}

			int audio_sample = sample_to_int( &(audio_samples[i][j]) );

			audio_sample/=MAX_USERS;

			mixed_audio_sample += audio_sample;
		}

		memcpy(&(final_audio[j]), &mixed_audio_sample, sizeof(int));
	}


	final_audio[SAMPLE] = 'A';
	final_audio[SAMPLE + 1] = '\n';
}

void* mix_audio(void* ptr)
{
	struct timespec ts;
    ts.tv_sec = -1;
    ts.tv_nsec = -1;
    struct timespec new_time;

	while(true)
	{
		//updata buffer every 10th of a second
		clock_gettime(CLOCK_MONOTONIC, &new_time);
        if((new_time.tv_sec - ts.tv_sec) *1000000000 + (new_time.tv_nsec - ts.tv_nsec) >= AUDIO_TIME)//off dome
        {
			pthread_mutex_lock(&main_audio_mutex);
			mix_audio_func();
			pthread_mutex_unlock(&main_audio_mutex);

			ts.tv_sec = new_time.tv_sec;
            ts.tv_nsec = new_time.tv_nsec;
		}
	}
}
