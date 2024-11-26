#include "scriptLib.sc";

sp = -0.5;
ep =  0.5;
min[] = { sp, sp, sp };
max[] = { ep, ep, ep };
plot3D.setArea(min[],max[]);
plot3D.setAxisFrame(1,0);
plot3D.setColor("Red");

sp[] = { -1, -1, -1 };
ep[] = {  1,  1,  1 };
plot3D.plotLine(sp[],ep[]);

//pause("plotLine");
plot3D.setColor("Blue");
circle(2,24);

plot3D.setColor("Green");
size = 2;
plot3D.plotRotate(RAD(45),"Y");
vec[] = { 2, 0, 0 };
plot3D.plotTranslate(vec[]);
wireCube(size);

plot3D.setColor("Red");
size = 4;
vec[] = { -1, 1, 0 };
plot3D.plotReset();
plot3D.plotTranslate(vec[]);
wireCube(size);

plot3D.disp();

//	円(半径,角数)
circle(r, n) {
    step = 2 * PI / n;
    i = 0;
    for (ang = 0; ang < 2 * PI; ang += step) {
        plist[i,0] = r * cos(ang);
        plist[i,1] = r * sin(ang);
        plist[i,2] = 0;
        i++;
    }
    plist[i,0] = r * cos(0);
    plist[i,1] = r * sin(0);
    plist[i,2] = 0;

    plot3D.plotPolyline(plist[,]);
}

faceCube(size) {
	s[0] = size / 2;
	s[1] = size / 2;
	s[2] = size / 2;
}

//	立方体(一辺の長さ)
wireCube(size) {
    sx = -size / 2;
    sy = -size / 2;
    sz = -size / 2;
    ex = size / 2;
    ey = size / 2;
    ez = size / 2;
    sp[] = { sx, sy, sz };
    ep[] = { ex, sy, sz };
    plot3D.plotLine(sp[],ep[]);
    sp[] = { sx, sy, sz };
    ep[] = { sx, ey, sz };
    plot3D.plotLine(sp[],ep[]);
    sp[] = { sx, sy, sz };
    ep[] = { sx, sy, ez };
    plot3D.plotLine(sp[],ep[]);

    sp[] = { sx, ey, ez };
    ep[] = { ex, ey, ez };
    plot3D.plotLine(sp[],ep[]);
    sp[] = { ex, sy, ez };
    ep[] = { ex, ey, ez };
    plot3D.plotLine(sp[],ep[]);
    sp[] = { ex, ey, sz };
    ep[] = { ex, ey, ez };
    plot3D.plotLine(sp[],ep[]);

    sp[] = { ex, sy, sz };
    ep[] = { ex, ey, sz };
    plot3D.plotLine(sp[],ep[]);
    sp[] = { ex, sy, sz };
    ep[] = { ex, sy, ez };
    plot3D.plotLine(sp[],ep[]);

    sp[] = { sx, ey, sz };
    ep[] = { sx, ey, ez };
    plot3D.plotLine(sp[],ep[]);
    sp[] = { sx, sy, ez };
    ep[] = { sx, ey, ez };
    plot3D.plotLine(sp[],ep[]);

    sp[] = { sx, sy, ez };
    ep[] = { ex, sy, ez };
    plot3D.plotLine(sp[],ep[]);
    sp[] = { sx, ey, sz };
    ep[] = { ex, ey, sz };
    plot3D.plotLine(sp[],ep[]);
}