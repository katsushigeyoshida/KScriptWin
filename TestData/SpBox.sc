#include "scriptLib.sc";


sp = -1;
ep =  01;
min[] = { sp, sp, sp };
max[] = { ep, ep, ep };
plot3D.setArea(min[],max[]);
plot3D.setAxisFrame(1,0);

plot3D.setColor("SandyBrown");
w = 4;
h = 8;
d = 4;
t = 0.5;
r = 1;
cx = w / 2;
cy = h / 5 * 3;

v[] = { -w / 2, -h /2, d/2};
plot3D.plotTranslate(v[]);
box(w, h, d, t, r, cx, cy);

plot3D.disp();


//  箱
box(w, h, d, t, r, cx, cy) {
    //  側板
    sidePlate(w, h, d, t);
    //  底板・天板
    updownPlate(w, h,d, t);
    //  背板
    backPlate(w, h, d, t);
    //  正面板
    frontPlate(w, h, d, t, r, cx, cy);
}

//  正面板
frontPlate(w, h, d, t, r, cx, cy) {
    plot3D.plotPush();
    plot3D.plotReset();
    plot3D.plotRotate(RAD(-90), "Y");
    v[] = { d - t, t, 0 };
    plot3D.plotTranslate(v[]);
    plot3D.plotPeekMulti();
    holePlate(w, h - 2 * t, t, r, cx, cy);
    plot3D.plotPop();
}


//  背板
backPlate(w, h, d, t) {
    plot3D.plotPush();
    plot3D.plotReset();
    plot3D.plotRotate(RAD(-90), "Y");
    v[] = { 0, t, 0 };
    plot3D.plotTranslate(v[]);
    plot3D.plotPeekMulti();
    plate(d, h - 2 * t, t);
    plot3D.plotPop();
}

//  側板
sidePlate(w, h, d, t) {
    plot3D.plotPush();
    plate(w, h, t);
    v[] = { 0, 0, -d - t };
    plot3D.plotTranslate(v[]);
    plate(w, h, t);
    plot3D.plotPop();
}

//  底板・天板
updownPlate(w, h,d, t) {
    plot3D.plotPush();
    //  底板
    plot3D.plotReset();
    plot3D.plotRotate(RAD(90), "X");
    plot3D.plotPeekMulti();
    plate(w, d, t);
    //  天板
    v[] = { 0, h - t, 0 };
    plot3D.plotTranslate(v[]);
    plate(w, d, t);
    plot3D.plotPop();
}

//  穴あき板(幅,高さ,厚さ,半径,穴位置x,y)
holePlate(w, h, t, r, cx, cy) {
    //  四角の側面
    p[,] = rectangleSide(w, h, t);
    plot3D.plotQuadeStrip(p[,]);
    //  円の側面
    cp[,] = circleSide(r, t);
    v[] = { cx, cy, 0 };
    cp[,] = plot3D.translate(cp[,], v[]);
    plot3D.plotQuadeStrip(cp[,]);
    //  板下部
    array.clear(p[,]);
    p[,] = rectangle(w, cy - r);
    plot3D.plotPolygon(p[,]);
    v[] = { 0, 0, t };
    p[,] = plot3D.translate(p[,], v[]);
    plot3D.plotPolygon(p[,]);
    //  板上部
    array.clear(p[,]);
    p[,] = rectangle(w, h - cy - r);
    v[] = { 0, h - r * 2 + t + t/1.7, 0 };
    p[,] = plot3D.translate(p[,], v[]);
    plot3D.plotPolygon(p[,]);
    v[] = { 0, 0, t};
    p[,] = plot3D.translate(p[,], v[]);
    plot3D.plotPolygon(p[,]);
    //  開口部
    array.clear(p[,]);
    array.clear(cp[,]);
    cp[,] = halfhollplate(r, w /2);
    v[] = { cx, cy, 0 };
    p[,] = plot3D.translate(cp[,], v[]);
    plot3D.plotQuadeStrip(p[,]);
    v[] = { 0, 0, t};
    p[,] = plot3D.translate(p[,], v[]);
    plot3D.plotQuadeStrip(p[,]);

    v[] = { cx, cy, 0 };
    p[,] = plot3D.rotate(cp[,], RAD(180), "Z");
    p[,] = plot3D.translate(p[,], v[]);
    plot3D.plotQuadeStrip(p[,]);
    v[] = { 0, 0, t};
    p[,] = plot3D.translate(p[,], v[]);
    plot3D.plotQuadeStrip(p[,]);
}

//  開口部の右半分データ
halfhollplate(r, w) {
    p[,] = arc(r, 8, -PI/2, PI/2);
    pcount = array.count(p[,0]);
    n = 0;
    for (i = 0; i < pcount; i++) {
        pos[n,0] = p[i,0];
        pos[n,1] = p[i,1];
        pos[n,2] = p[i,2];
        n++;
        pos[n,0] = w;
        pos[n,1] = p[i,1];
        pos[n,2] = p[i,2];
        n++;
    }
    return pos[,];
}

//  板(幅、高さ、暑さ)
plate(w, h, t) {
    //  外枠
    p[,] = rectangleSide(w, h, t);
    plot3D.plotQuadeStrip(p[,]);
    //  下面
    array.clear(p[,]);
    p[,] = rectangle(w, h);
    array.reverse(p[,]);
    plot3D.plotPolygon(p[,]);
    //  上面
    p[,] = rectangle(w, h);
    v[] = { t, 0, 0};
    p[,] = plot3D.translate(p[,], v[]); 
    array.reverse(p[,]);
    plot3D.plotPolygon(p[,]);
}

//  長方形の座標データ
rectangle(w, h) {
    p[0,] = { 0, 0, 0 };
    p[1,] = { w, 0, 0 };
    p[2,] = { w, h, 0 };
    p[3,] = { 0, h, 0 };
    return p[,];
}

//  長方形の側面座標データ
rectangleSide(w, h, t) {
    p[0,] = { 0, 0, 0 };
    p[1,] = { 0, 0, t };
    p[2,] = { w, 0, 0 };
    p[3,] = { w, 0, t };
    p[4,] = { w, h, 0 };
    p[5,] = { w, h, t };
    p[6,] = { 0, h, 0 };
    p[7,] = { 0, h, t };
    p[8,] = { 0, 0, 0 };
    p[9,] = { 0, 0, t };
    return p[,];
}

//  円の側面の座標データ
circleSide(r, t) {
    c[,] = circle(r, 16);
    n =  array.count(c[,]) / 3;
    n = floor(n);
    for (i = 0; i < n; i++) {
        low = i * 2;
        cp[low, 0] = c[i,0] + cx;
        cp[low, 1] = c[i,1] + cy;
        cp[low, 2] = t;
        low++;
        cp[low, 0] = c[i,0] + cx;
        cp[low, 1] = c[i,1] + cy;
        cp[low, 2] =  0;
    }
    low = i * 2;
    cp[low, 0] = c[0,0] + cx;
    cp[low, 1] = c[0,1] + cy;
    cp[low, 2] = t;
    low++;
    cp[low, 0] = c[0,0] + cx;
    cp[low, 1] = c[0,1] + cy;
    cp[low, 2] =  0;
    return cp[,];
}

//  円の座標データ(半径,分割数)
circle(r, n) {
    p[,] = arc(r, n, 0, 2 * PI);
    return p[,];
}

//  円弧の座標データ(半径,分割数,始角,終角)
arc(r, n, sa, ea) {
   step = (ea - sa) / n;
    i = 0;
        z = 0;
    for (ang  = sa; ang < ea; ang += step) {
        x = r * cos(ang);
        y = r * sin(ang);
        pos[i, 0] = x;
        pos[i, 1] = y;
        pos[i, 2] = z;
        i++;
    }
    if ((ea - sa) < 2 * PI) {
        x = r * cos(ang);
        y = r * sin(ang);
        pos[i, 0] = x;
        pos[i, 1] = y;
        pos[i, 2] = z;
    }
    return pos[,];
}

