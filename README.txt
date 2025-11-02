This is incomplete 3d chatting program, in its finished state users will be able to float around as an avatar and proximity chat.

[DEMO VIDEO version 1](https://github.com/Jonathan-Git-Hub-dev/3D_chat/blob/master/demo_3d.mp4)
[JWT_Documentation](https://github.com/Jonathan-Git-Hub-dev/3D_chat/blob/master/demo_3d.mp4)
[JWT_Documentation](https://www.jwt.io/introduction)

How the rendering works:
The user is described by a point in 3d space and 2 angles (z_angle, xy_angle). these two angles are used to create a vector called the line of sight.

With this information we can.

Define a plane that holds this point with a normal vector the same as our line of sight vector.
Find where our line of sight intersects this plane and call this the centre of the plane.
Find vectors that define "horizontal" and "vertical" movement in this plane.
Find how many units weather they be positive or negative our target point is away from centre.
use our field of view angles/ ratio to translate these units onto our screen.
Do these calculations for the other two points of a face, and every face on the screen.




