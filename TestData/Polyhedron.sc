#include "scriptLib.sc";

//	正多角形
sp = -0.5;
ep =  0.5;
min[] = { sp, sp, sp };
max[] = { ep, ep, ep };
plot3D.setArea(min[],max[]);
plot3D.setAxisFrame(1,0);
plot3D.setColor("Red");


//	正四面体
r = 1;
n = 3;
plist[,] = polygon(r,n);
printArray2(plist[,]);
h = 

plot3D.plotPolyline(plist[,]);

plot3D.disp();

polygon(r,n) {
	ang = 0;
	dang = 2 * PI / n;
	i = 0;
	while (ang <= 2*PI) {
		p[i,0] = r * cos(ang);
		p[i,1] = r * sin(ang);
		p[i,2] = 0;
		ang += dang;
		i++;
	}
	return p[,];
}
		
	
