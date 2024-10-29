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
