#include <stdbool.h>
#include <string.h>
#include <stdio.h>
#include <stdlib.h>
#include <limits.h>

#include "j_globals.h"


//debugging function
void printBinary(int num)
{
    int num_bits = sizeof(int) * CHAR_BIT;

    for (int i = num_bits - 1; i >= 0; i--)
    {
        printf("%d", (num >> i) & 1);
    }
    printf("\n");
}
