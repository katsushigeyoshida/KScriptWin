//  Matrix Test
println("単位行列の作成");
a[,] = matrix.unit(3);
println(a[,]);

a[,] = {
    { 1, 2, 3 },
    { 4, 5, 6 },
    { 7, 8, 9 }
}
e[,] = {
    { 10, 20, 30 },
    { 40, 50, 60 },
    { 70, 80, 90 }
}
b[,] = {
    { 1, 2 },
    { 4, 5 },
    { 7, 8 }
}
c[,] = {
    { 1 },
    { 2 },
    { 3 }
}
print("行列 a \n");
println(a[,]);
print("行列 b \n");
println(b[,]);
print("転置行列 a\n");
d[,] = matrix.transpose(a[,]);
println(d[,]);
print("行列の積 a x b\n");
array.clear(d[,]);
d[,] = matrix.multi(a[,], b[,]);
println(d[,]);
print("行列の和 a + e\n");
array.clear(d[,]);
d[,] = matrix.add(a[,], e[,]);
println(d[,]);
print("逆行列 a^-1\n");
array.clear(d[,]);
d[,] = matrix.inverse(a[,]);
println(d[,]);
d[,] = matrix.inverse(d[,]);
println(d[,]);

