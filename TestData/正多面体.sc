#include "scriptLib.sc";
//	https://ooya-takemasa.thick.jp/Suugaku/Seitamentai.pdf
//	https://qiita.com/ikiuo/items/f5905c353858fc43e597

//	正多面体 Polyhedron
sp = -0.5;
ep =  0.5;
min[] = { sp, sp, sp };
max[] = { ep, ep, ep };
plot3D.setArea(min[],max[]);
plot3D.setAxisFrame(1,0);
plot3D.setColor("Green");

title = "図形の種類";
menu[] = {  "正四面体wire", "正四面体face",
			"正六面体wire", "正六面体face",
			"正八面体wire", "正八面体face",
			"正十二面体wire","正十二面体face",
			"正二十面体wire","正二十面体face"
		};
menuNo = menuSelect(menu[], title);

if (menuNo == 0) tetrahedron(0);
else if (menuNo == 1) tetrahedron(1);
else if (menuNo == 2) hexahedron(0);
else if (menuNo == 3) hexahedron(1);
else if (menuNo == 4) octahedron(0);
else if (menuNo == 5) octahedron(1);
else if (menuNo == 6) dodecahedron(0);
else if (menuNo == 7) dodecahedron(1);
else if (menuNo == 8) icosahedron(0);
else if (menuNo == 9) icosahedron(1);

plot3D.disp();

//	正四面体
tetrahedron(surface){
    r = 3;
    n = 3;
    h = r * sqrt(2);
    plist[,] = polygon(r,n);
    plist[4,] = { 0,0,h};
    vec[] = { 0,0,-r/2};
    plist[,] = plot3D.translate(plist[,],vec[]);
    if (surface ==1) {
        p[0,] = plist[0,];
        p[1,] = plist[1,];
        p[2,] = plist[2,];
        plot3D.plotPolygon(p[,]);
        for (i = 0; i < 3; i++) {
            p[0,] = plist[i,];
            p[1,] = plist[i+1,];
            p[2,] = plist[4,];
            plot3D.plotPolygon(p[,]);
        }
    } else {
        for (i = 0; i < 3; i++)
        	plot3D.plotLine(plist[i,],plist[i+1,]);
        for (i = 0; i < 3; i++)
        	plot3D.plotLine(plist[i,],plist[4,]);
    }
}

//	正六面体 hexahedron
hexahedron(surface) {
	r = 3;
	n = 4;
	h = sqrt(2) / 2 * r;
	plist[,]= polygon(r,n);
	if (surface != 1)
		plist[4,] = plist[0,];
	vec[] = { 0, 0, h };
	uplist[,] = plot3D.translate(plist[,], vec[]);
	vec[] = { 0, 0, -h };
	dplist[,] = plot3D.translate(plist[,], vec[]);
	if (surface !=1) {
	    plot3D.plotPolyline(uplist[,]);
	    plot3D.plotPolyline(dplist[,]);
	    for (i= 0; i < 4; i++)
	    	plot3D.plotLine(uplist[i,],dplist[i,]);
    } else {
    	plot3D.plotPolygon(uplist[,]);
    	plot3D.plotPolygon(dplist[,]);
    	for (i = 0; i < 4; i++) {
    		p[0,] = uplist[i,];
    		p[1,] = dplist[i,];
    		j = (i + 1) % 4;
    		p[2,] = dplist[j,];
    		p[3,] = uplist[j,];
    		plot3D.plotPolygon(p[,]);
    	}
    }
}

//	正八面体 octahedron
octahedron(surface){
	r = 3;
	n = 4;
	h = r;
	plist[,]= polygon(r,n);
	count = array.count(plist[,0]);
	println(count);
	up[] = { 0, 0, h };
	dp[] = { 0, 0,-h };
	if (surface != 1) {
		plot3D.plotPolyline(plist[,]);
		for (i = 0; i < count - 1; i++) {
			plot3D.plotLine(plist[i,], up[]);
			plot3D.plotLine(plist[i,], dp[]);
		}
	} else {
		for (i = 0; i < count - 1; i++) {
			p[0,] = plist[i,];
			p[1,] = plist[i + 1,];
			p[2,] = up[];
			plot3D.plotPolygon(p[,]);
			p[2,] = dp[];
			plot3D.plotPolygon(p[,]);
		}		
	}
}

//	正十二面体 dodecahedron
dodecahedron(surface){
	phai = (1 + sqrt(5))/ 2;
	phai2 = phai * phai;
	p[0,] = { 0, -1, -phai2 };
	p[1,] = { 0,  1, -phai2 };
	p[2,] = { 0, -1,  phai2 };
	p[3,] = { 0,  1,  phai2 };

	p[4,] = { -1, -phai2, 0 };
	p[5,] = {  1, -phai2, 0 };
	p[6,] = { -1,  phai2, 0 };
	p[7,] = {  1,  phai2, 0 };
	
	p[8,] =  { -phai2, 0, -1 };
	p[9,] =  { -phai2, 0,  1 };
	p[10,] = {  phai2, 0, -1 };
	p[11,] = {  phai2, 0,  1 };

	p[12,] = { -phai, -phai,  -phai };
	p[13,] = { -phai, -phai,   phai };
	p[14,] = { -phai,  phai,  -phai };
	p[15,] = { -phai,  phai,   phai };

	p[16,] = {  phai, -phai,  -phai };
	p[17,] = {  phai, -phai,   phai };
	p[18,] = {  phai,  phai,  -phai };
	p[19,] = {  phai,  phai,   phai };
	
	a[,] = {{  0, 1,18,10,16 },
			{  1, 0,12, 8,14 },
			{  2, 3,15, 9,13 },
			{  3, 2,17,11,19 },
			{  4, 5,17, 2,13 },
			{  5, 4,12, 0,16 },
			{  6, 7,18, 1,14 },
			{  7, 6,15, 3,19 },
			{  8, 9,15, 6,14 },
			{  9, 8,12, 4,13 },
			{ 10,11,17, 5,16 },
			{ 11,10,18, 7,19 }};

	for (i = 0; i < 12; i++) {
		for (j = 0; j < 5; j++) {
			n = a[i,j];
			plist[j,] = p[n,];
		}
		polyface(plist[,],surface);
	}	
}

//	正二十面体 icosahedron
icosahedron(surface) {
	phai = (1 + sqrt(5))/ 2;
	phai2 = phai * phai;
	p[0,] = { 0, -1, -phai };
	p[1,] = { 0,  1, -phai };
	p[2,] = { 0, -1,  phai };
	p[3,] = { 0,  1,  phai };

	p[4,] = { -phai,  0,  -1 };
	p[5,] = { -phai,  0,   1 };
	p[6,] = {  phai,  0,  -1 };
	p[7,] = {  phai,  0,   1 };

	p[8,] =  { -1, -phai,  0 };
	p[9,] =  {  1, -phai,  0 };
	p[10,] = { -1,  phai,  0 };
	p[11,] = {  1,  phai,  0 };

	a[,] = {{ 0,1, 6 }, { 1,0,4 }, { 2, 3, 5 }, { 3, 2, 7 },
			{ 4,5,10 }, { 5,4,8 }, { 6, 7, 9 }, { 7, 6,11 },
			{ 8,9, 2 }, { 9,8,0 }, {10,11, 1 }, {11,10, 3 },
			{ 0,6, 9 }, { 0,8,4 }, { 1, 4,10 }, { 1,11, 6 },
			{ 2,5, 8 }, { 2,9,7 }, { 3, 7,11 }, { 3,10, 5 }};

	for (i = 0; i < 20; i++) {
		for (j = 0; j < 3; j++) {
			n = a[i,j];
			plist[j,] = p[n,];
		}
		polyface(plist[,],surface);
	}	
}

//	ワイヤフレーム/フェース表示　　　　　　　　　　　
polyface(plist[,],surface) {
	if (surface == 1)
		plot3D.plotPolygon(plist[,]);
	else {
		c = array.count(plist[,0]);
		plist[c,] = plist[0,];
		plot3D.plotPolyline(plist[,]);
	}
}

//	多角形データ
polygon(r,n) {
	ang = 0;
	eang = 2 * PI + ang;
	dang = 2 * PI / n;
	i = 0;
	while (ang <= eang) {
		x = r * cos(ang);
		y = r * sin(ang);
		p[i,0] = x;
		p[i,1] = y;
		p[i,2] = 0;
		ang += dang;
		i++;
	}
	return p[,];
}
		
	
