//	連立方程式
//	a11x1 + a12x2 + a13x3 = b1
//	a21x1 + a22x2 + a23x3 = b2
//	a31x1 + a32x2 + a33x3 = b3
//	    a11 a12 a13        x1      b1
//	A = a21 a22 a23    x = x2  b = b2
//	    a31 a32 a33        x3      b3
//	Ax = b


// 係数の定義
//	a1*x + b1*y = c1
//	a2*x + b2*y = c1

//a1 = 2; b1 = 3; c1 = 6;
//a2 = 1; b2 = 2; c2 = 4;
a1 = 22; b1 = 1; c1 = 1817;
//a2 = 761; b2 = 1; c2 = 30973;
a2 = 321; b2 = 1; c2 = 12435;
a2 = 69; b2 = 1; c2 = 1967;
println(a1," ",b1," ",c1);
println(a2," ",b2," ",c2);
// 行列式の計算
determinant = a1 * b2 - a2 * b1;

if (determinant == 0)  {
	println("解が存在しません（行列式がゼロです）。");
} else {
	// クラメルの公式を使用して解を求める
    x = (c1 * b2 - c2 * b1) / determinant;
    y = (a1 * c2 - a2 * c1) / determinant;

    println("解: x = ",x, "  y = ",y);
}

d1 = a1*x+b1*y;
d2 = a2*x+b2*y;
println(d1," ",d2);
