#include "scriptLib.sc";
//  3次元関数の表示
sp = -1;
ep =  1;
min[] = { sp, sp, sp };
max[] = { ep, ep, ep };
plot3D.setArea(min[],max[]);
plot3D.setAxisFrame(1,1);
plot3D.setColor("Red");

sp *= 4;
ep *= 4;
stepcount = 30;
xstep = (ep - sp) / stepcount;
zstep = (ep - sp) / stepcount;
startTime();
for (x = sp; x <= ep; x += xstep) {
    n = 0;
    for (z = sp; z <= ep; z += zstep) {
        y = func(x, z);
        p[n, 0] = x;
        p[n, 1] = y;
        p[n, 2] = z;
        n++;
    }
    plot3D.plotPolyline(p[,]);
    plot3D.disp();
    array.clear(p[,]);
}
println(lapTime());


func(x, z) {
    c = sqrt(x * x + z * z);
    return cos(c) + cos(3 * c);
}



