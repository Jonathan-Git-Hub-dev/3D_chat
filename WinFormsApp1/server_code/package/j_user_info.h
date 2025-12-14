#define MAX_USERS 4

#include <stdbool.h>

struct position {
	double x;
        double y;
        double z;
};

struct colour {
	int red;
        int green;
        int blue;
};

struct user_data{

	struct position user_position;

	struct colour user_colour;

	int asset;

	int angle;

	bool online;
};
void print_user_data(struct user_data *instance);

void create_user_data(struct user_data *instance);

void update_user_data(struct user_data *instance, struct position p, struct colour c, int asset, int angle);

void clear_user_data(struct user_data *instance);
