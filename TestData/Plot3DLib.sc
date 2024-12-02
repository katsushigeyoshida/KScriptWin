//  穴付き板表示(幅､高さ,板厚,半径,穴位置x,穴位置Y)
holePlate(w,h,t,r,cx,cy) {
    n = 16;
    v[] ={ 0, 0, t };
    vc[] = {cx,cy,0 };
    rect[,] = rectangle(w,h);
    cir[,] = circle(r,n);
    cir[,] = plot3D.translate(cir[,],vc[]);
    p[,] = plot3D.holePlateQuads(rect[,],cir[,]);
    array.reverse(p[,]);
    plot3D.plotQuads(p[,]);
    p[,]= plot3D.translate(p[,],v[]);
    array.reverse(p[,]);
    plot3D.plotQuads(p[,]);
    array.clear(p[]);
    p[,] = plot3D.polygonSideQuadStrip(rect[,],t);
    array.reverse(p[,]);
    plot3D.plotQuadeStrip(p[,]);
    array.clear(p[]);
    p[,] = plot3D.polygonSideQuadStrip(cir[,],t);
    array.reverse(p[,]);
    plot3D.plotQuadeStrip(p[,]);
}

//  板表示(幅、高さ、厚さ)v)
plate(w, h, t) {
    //  下面
    array.clear(p[,]);
    rect[,] = rectangle(w, h);
    array.reverse(rect[,]);
    plot3D.plotPolygon(rect[,]);
    //  上面
    array.reverse(rect[,]);
    v[] = { 0, 0, t};
    p[,] = plot3D.translate(rect[,], v[]);
    p[,] = plot3D.rotate(p[,],angle,axis);
    plot3D.plotPolygon(p[,]);
    //  外枠
    array.reverse(rect[,]);
    p[,] = plot3D.polygonSideQuadStrip(rect[,],-t);
    plot3D.plotQuadeStrip(p[,]);
}

//  円データ(半径,角数)
circle(r,n) {
    if (n < 1) return pos[,];
    step = 2 * PI / n;
    i = 0;
    z = 0;
    for (ang  = 0; ang < 2 * PI; ang += step) {
        x = r * cos(ang);
        y = r * sin(ang);
        pos[i, 0] = x;
        pos[i, 1] = y;
        pos[i, 2] = z;
        i++;
    }
    return pos[,];
}

//  長方形データ(幅,高さ)(x,y,0)
rectangle(w, h) {
    w /= 2;
    h /= 2;
    p[0,] = { -w, -h, 0 };
    p[1,] = {  w, -h, 0 };
    p[2,] = {  w,  h, 0 };
    p[3,] = { -w,  h, 0 };
    return p[,];
}