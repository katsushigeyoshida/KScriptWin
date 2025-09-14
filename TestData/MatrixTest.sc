//  Matrix Test
println("単位行列の作成");
a[,] = matrix.unit(3);
printArray2(a[,]);

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
printArray2(a[,]);
print("行列 b \n");
printArray2(b[,]);
print("転置行列 a\n");
d[,] = matrix.transpose(a[,]);
printArray2(d[,]);
print("行列の積 a x b\n");
array.clear(d[,]);
d[,] = matrix.multi(a[,], b[,]);
printArray2(d[,]);
print("行列の和 a + e\n");
array.clear(d[,]);
d[,] = matrix.add(a[,], e[,]);
printArray2(d[,]);
print("逆行列 a^-1\n");
array.clear(d[,]);
d[,] = matrix.inverse(a[,]);
printArray2(d[,]);
d[,] = matrix.inverse(d[,]);
printArray2(d[,]);

//  最小二乗法

//  2D配列表示
printArray2(array[,]) {
    size = array.count(array[,]);
//    print("サイズ : ",size, "\n");
    count = 0;
    i = 0;
    while (count < size) {
        rowsize = array.count(array[i,]);
//        print(i, " : ");
        for (j = 0; j < rowsize; j++) {
            print(array[i,j], " ");
            count++;
        }
        print();
        i++;
    }
    print();
}
