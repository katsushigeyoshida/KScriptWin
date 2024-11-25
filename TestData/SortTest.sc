//  ソートテスト
data[] = { 6, 9, 12, 7, 2, 23, 10, 4 };
println("ソートデータ");
arrayPrint(data[]);
menu[] = {
    "単純選択法", "バブルソート", "単純挿入法",
    "シェーカーソート", "単純挿入法2", "シェルソート",
    "クイックソート"
};
title = "ソートの種類";
menuNo = menuSelect(menu[], title);
println(menuNo);
if (menuNo == 0) simpleSelect(data[]);
else if (menuNo == 1) bubleSort(data[]);
else if (menuNo == 2) simpleInsert(data[]);
else if (menuNo == 3) shakerSort(data[]);
else if (menuNo == 4) simpleInsert2(data[]);
else if (menuNo == 5) shellSort(data[]);
else if (menuNo == 6) quickSort(data[]);


simpleSelect(a[]) {
    sp = 0;
    ep = array.count(a[]);
    println("単純選択法 ",ep);
    count = 0;
    n = ep;
    for ( i = sp; i < n; i = i + 1) {
        k = sp;
        for (j = sp; j < n; j = j + 1) {
            count++;
            if (a[k] > a[j]) k = j;
        }
        b[i] = a[k];
        a[k] = 9999;
        print("[",count,"] ")
        arrayPrint(b[]);
    }
}

bubleSort(a[]) {
    println("バブルソート");
    sp = 0;
    ep = array.count(a[]);
    count = 0;
    for ( i = sp; i < ep; i = i +1){
        for ( j = sp; j < ep - 1; j = j + 1) {
            count++;
            if (a[j] > a[j + 1]){
                w = a[j];
                a[j] = a[j + 1];
                a[j + 1] = w;
            }
        }
        print("[",count,"] ")
        arrayPrint(a[]);
    }
}

simpleInsert(a[]) {
    println("単純挿入法(1列目作業データ)");
    sp = 0;
    ep = array.count(a[]);
    count = 0;
    for ( i = sp; i < ep; i = i +1){
        count++;
        b[sp] = a[i];
        k = i;
        while(b[sp] < b[k]) {
            count++;
            b[k + 1] = b[k];
            k = k - 1;
        }
        b[k + 1] = b[sp];
        print("[",count,"] ")
        arrayPrint(b[]);
    }
}

shakerSort(a[]) {
    println("シェーカーソート");
    n = array.count(a[]);
    count = 0;
    shift = 0;
    left = 0;
    right = n - 1;
    while (left < right) {
        for (i = left; i < right; i++) {
            count++;
            if (a[i] > a[i+1]) {
                t = a[i];
                a[i] = a[i+1];
                a[i+1] = t;
                shift = i;
            }
        }
        print("[",count,"] ")
        arrayPrint(a[]);
        right = shift;
        for (i = right; i > left; i--) {
            count++;
            if (a[i] < a[i-1]) {
                t = a[i];
                a[i] = a[i-1];
                a[i-1] = t;
                shift = i;
            }
        }
        left = shift;
        print("[",count,"] ")
        arrayPrint(a[]);
    }
}

simpleInsert2(a[]) {
    println("単純挿入ソート");
    n = array.count(a[]) - 1;
    count = 0;
    for (i = 1; i < n; i++) {
        for (j = i - 1; j >= 0; j--) {
            count++;
            if (a[j] > a[j+1]) {
                t = a[j];
                a[j] = a[j+1];
                a[j+1] = t;
            } else {
                break;
            }
        }
        print("[",count,"] ")
        arrayPrint(a[]);
    }
}

shellSort(a[]) {
    println("シェルソート");
    n = array.count(a[]) - 1;
    gap = floor(n / 2);
    count = 0;
    while (gap > 0) {
        for (k = 0; k < gap; k++) {
            for (i = k + gap; i < n; i+=gap) {
                for (j = i - gap; j >= k; j-=gap) {
                    count++;
                    if (a[j] > a[j+gap]) {
                        t = a[j];
                        a[j] = a[j+gap];
                        a[j+gap] = t;
                    } else {
                        break;
                    }
                }
            }
            print("[",count,"] ")
            arrayPrint(a[]);
        }
        gap = floor(gap / 2);
    }
}

quickSort(a[]) {
    println("クイックソート");
    n = array.count(a[]) - 1;
    a[] = quick(a[], 0, n);
}

quick(a[], left, right) {
    if (left < right) {
        print(left, " ", right, " : ");
        arrayPrint(a[]);
        s = a[left];
        i = left;
        j = right + 1;
        while (1) {
            while (a[++i] < s) ;
            while (a[--j] > s) ;
            if (i >= j) break;
            t = a[i];
            a[i] = a[j];
            a[j] = t;
        }
        a[left] = a[j];
        a[j] = s;
        
        a[] = quick(a[], left, j - 1);
        a[] = quick(a[], j + 1, right);
    }
    return a[];
}

arrayPrint(b[]) {
    i = 0;
    ep = array.count(b[]);
    while (i < ep) {
        print(b[i], " ");
        i = i + 1;
    }
    print();
}

