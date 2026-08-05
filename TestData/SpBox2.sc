#include "scriptLib.sc";
#include "Plot3DLib.sc";

w = 2; h = 4; d = 2; t = 0.2; r = 0.5; cx = 0; cy = 1;
para[,] = {{"幅", w}, {"高さ", h}, {"奥行", d}, {"板厚",t},
	{"穴半径", r}, {"穴位置x", cx}, {"穴位置y", cy}};
para[,] = inputBoxMulti(para[,]);
if (array.count(para[,]) == 0)
	exit();
println(para[,]);

w = para[0,1];
h = para[1,1];
d = para[2,1];
t = para[3,1];
r = para[4,1];
cx = para[5,1];
cy = para[6,1];

println(w," ",h," ",d," ",t," ",r," ",cx," ",cy);
sp = -0.5;
ep =  0.5;
min[] = { sp, sp, sp };
max[] = { ep, ep, ep };
plot3D.setArea(min[],max[]);
plot3D.setAxisFrame(1,0);
plot3D.setColor("SandyBrown");

p[,] = box(w, h, d, t, r, cx, cy);
plot3D.dataClear();
plot3D.plotQuads(p[,]);
plot3D.disp();

box(w, h, d, t, r, cx,cy) {
    //  側板
    array.clear(p[,]);
    side[,] = plateQuads(d,h,t);
    side[,] = plot3D.rotate(side[,],RAD(90),"Y");
    vec[] = { w/2+t, 0, 0 };
    p[,] = plot3D.translate(side[,],vec[]);
    vec[] = { -w/2, 0, 0 };
    leftside[,] = plot3D.translate(side[,],vec[]);
    p[,] = array.concat(p[,],leftside[,]);
    //  底板・天板
    top[,] = plateQuads(w+2*t,d,t);
    top[,] = plot3D.rotate(top[,],RAD(90),"X");
    vec[] = { 0, h/2, 0 };
    top[,] = plot3D.translate(top[,],vec[]);
    p[,] = array.concat(p[,],top[,]);
    vec[] = { 0, -h-t, 0 };
    bottom[,] = plot3D.translate(top[,],vec[]);
    p[,] = array.concat(p[,],bottom[,]);
	//	背板
    back[,] = plateQuads(w,h,t);
    vec[] = { 0, 0, -d/2 };
    back[,] = plot3D.translate(back[,],vec[]);
    p[,] = array.concat(p[,],back[,]);
	//	正面板
    front[,] = holePlateQuads(w, h, t, r, cx, cy);
    vec[] = { 0, 0, d/2-t };
    front[,] = plot3D.translate(front[,],vec[]);
    p[,] = array.concat(p[,],front[,]);
    return p[,];
}
