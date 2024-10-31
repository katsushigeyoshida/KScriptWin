//  Matrix Test
a[,] = {
    { 1, 2, 3 },
    { 4, 5, 6 },
    { 7, 8, 9 }
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
d[,] = matrixTranspose(a[,]);
printArray2(d[,]);
print("行列の積 a x b\n");
clear(d[,]);
d[,] = matrixMulti(a[,], b[,]);
printArray2(d[,]);
print("逆行列 a^-1\n");
clear(d[,]);
d[,] = matrixInverse(a[,]);
printArray2(d[,]);

//  最小二乗法
//  2D配列表示
printArray2(array[,]) {
    size = count(array[,]);
//    print("サイズ : ",size, "\n");
    count = 0;
    i = 0;
    while (count < size) {
        rowsize = count(array[i,]);
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
