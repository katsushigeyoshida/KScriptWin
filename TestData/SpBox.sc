#include "scriptLib.sc";
#include "Plot3DLib.sc";
sp = -2;
ep =  2;
min[] = { sp, sp, sp };
max[] = { ep, ep, ep };
plot3D.setArea(min[],max[]);
plot3D.setAxisFrame(1,0);

//	スピーカボックス
w = 10;
h = 15;
d = 8;
r = 3;
cx = 0;
cy = 2;
t = 0.5;
plot3D.setColor("SandyBrown");

v[] = { 0,0,5};
plot3D.plotTranslate(v[]);
plot3D.plotPush();
//	前面
holePlateDisp(w,h,t,r,cx,cy);
//	背面
v[] = { 0,0,-d};
plot3D.plotTranslate(v[]);
plateDisp(w,h,t);
//	左側面
plot3D.plotReset();
plot3D.plotRotate(RAD(90),"Y");
v[] = { -w/2,0,(-d+t)/2};
plot3D.plotTranslate(v[]);
plot3D.plotPeekMulti();
plateDisp(d+t,h,t);
//	右側面
v[] = { w+t,0,0};
plot3D.plotTranslate(v[]);
plateDisp(d+t,h,t);
//	天板
plot3D.plotReset();
plot3D.plotRotate(RAD(90),"X");
v[] = { 0,h/2,(-d+t)/2};
plot3D.plotTranslate(v[]);
plot3D.plotPeekMulti();
plateDisp(w+2*t,d+t,t);
//	底板
v[] = { 0,-h-t,0};
plot3D.plotTranslate(v[]);
plateDisp(w+2*t,d+t,t);

plot3D.disp();