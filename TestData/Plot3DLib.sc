//  穴付き板表示(幅､高さ,板厚,半径,穴位置x,穴位置Y)
holePlateDisp(w,h,t,r,cx,cy) {
    n = 16;
    v[] ={ 0, 0, t };
    vc[] = {cx,cy,0 };
    rect[,] = rectanglePolygon(w,h);
    cir[,] = circlePolygon(r,n);
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

//  板表示(幅,高さ,厚さ)
plateDisp(w, h, t) {
    //  下面
    array.clear(p[,]);
    rect[,] = rectanglePolygon(w, h);
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

//  板データ(幅,高さ,板厚)(QUADS)
plateQuads(w, h, t) {
   rect[,] = rectanglePolygon(w, h);
    //  外枠
   side[,] = plot3D.polygonSideQuads(rect[,],t);
    //  上面
   v[] = { 0, 0, t };
   recttop[,] = plot3D.translate(rect[,],v[]);
    top[,] = plot3D.holePlateQuads(recttop[,]);
    p[,] = array.concat(side[,],top[,]);
    //  下面
    rect[,] = array.reverse(rect[,]);
    bottom[,] = plot3D.holePlateQuads(rect[,]);
    p[,] = array.concat(p[,],bottom[,]);
    return p[,];
}

//	穴付き平板データ(幅,高さ,板厚,半径,穴位置X,穴位置Y)(QUADS)
hallPlateQuads(w, h, t, r, cx, cy) {
   // 四角の側面
   rect[,] = rectanglePolygon(w, h);
   rectside[,] = plot3D.polygonSideQuads(rect[,],t);
    // 円の側面
    cir[,] = circlePolygon(r, 16);
    v[] = { cx, cy, 0 };
    cir[,] = plot3D.translate(cir[,],v[]);
   cirside[,] = plot3D.polygonSideQuads(cir[,],t);
   p[,] = array.concat(rectside[,],cirside[,]);
   // 後面
   front[,] = plot3D.holePlateQuads(rect[,],cir[,]);
   front[,] = array.reverse(front[,]);
   p[,] = array.concat(p[,],front[,]);
   // 前面
    v[] = { 0, 0, t };
    brect[,] = plot3D.translate(rect[,],v[]);
   back[,] = plot3D.holePlateQuads(brect[,],cir[,]);
   p[,] = array.concat(p[,],back[,]);
   return p[,];
}

// 四角形データ(幅,高さ)(Polygon)
rectanglePolygon(w, h) {
   w /= 2;
   h /= 2;
    p[0,] = { -w, -h, 0 };
    p[1,] = {  w, -h, 0 };
    p[2,] = {  w,  h, 0 };
    p[3,] = { -w,  h, 0 };
   return p[,];
}

// 円データ(半径,角数)(Polygon)
circlePolygon(r, n) {
    step = 2 * PI / n;
    i = 0;
    for (ang  = 0; ang < (2 * PI); ang += step) {
        pos[i, 0] = r * cos(ang);
        pos[i, 1] = r * sin(ang);
        pos[i, 2] = 0;
        i++;
    }
    return pos[,];
}

//	球体データ(半径,角数)(QUADS)
sphereQuads(r,n) {
	if (n <= 0)
		n = 32;
	a[,] = arc(r,n,0,PI);
	cl[,] = { { 0,0,0}, {1,0,0}};
	divAng = 2 * PI / n;
	sa = 0; ea = 2 * PI;
	p[,] = plot3D.polylinRotateQuads(a[,],cl[,],divAng,sa,ea);
	return p[,];
}

//	円弧座標データ(半径,角数,開始角,終了角)(Polygon)
arcPolygon(r,n,sa,ea) {
    step = 2 * PI / n;
    i = 0;
    z = 0;
    for (ang = sa; ang - step < ea; ang += step) {
    	if (ea < ang) ang = ea;
        x = r * cos(ang);
        y = r * sin(ang);
        pos[i, 0] = x;
        pos[i, 1] = y;
        pos[i, 2] = z;
        i++;
    }
    return pos[,];
}
