//  Script Library
//  配列表示
printArray(array[]) {
    start = 0;
    size = count(array[]);
    for (i = start; i < size; i = i + 1) {
        print(array[i], " ");
    }
    print();
}
//  2D配列表示
printArray2(array[,]) {
    size = count(array[,]);
    count = 0;
    i = 0;
    while (count < size) {
        rowsize = count(array[i,]);
        for (j = 0; j < rowsize; j++) {
            print(array[i,j], " ");
            count++;
        }
        print();
        i++;
    }
}


//	タートルグラフィック
turtleSetpoint(x, y) {
	g_TurtleLPX = x;
	g_TurtleLPY = y;
}

turtleSetAngle(ang) {
	g_TurtleAngle = ang;
}

turtleMoveto(x, y) {
	plotLine(g_TurtleLPX, g_TurtleLPY, x, y);
	g_TurtleLPX = x;
	g_TurtleLPY = y;
}

turtleMove(l) {
	ang = RAD(g_TurtleAngle);
	x = g_TurtleLPX + l * cos(ang);
	y = g_TurtleLPY + l * sin(ang);
	plotLine(g_TurtleLPX, g_TurtleLPY, x, y);
	g_TurtleLPX = x;
	g_TurtleLPY = y;
}

turtleTurn(angle) {
	g_TurtleAngle = (g_TurtleAngle + angle) % 360;
}

turtleInit(){
	g_TurtleAngle = 0;	//	degree
	g_TurtleLPX = 0;
	g_TurtleLPY = 0;
}