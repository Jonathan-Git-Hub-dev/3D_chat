main: main.c
        ulimit -c unlimited
        echo 'core' | sudo tee /proc/sys/kernel/core_pattern
        gcc -O0 -g main.c package/j_socket.c package/j_thread.c package/j_user_info.c package/j_temp.c package/j_audio.c package/j_globals.c -l pthread