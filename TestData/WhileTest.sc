//	インクリメントテスト
println("Increment Test");
n= 1;
println(n);
println(n += 1);
println(n++);
println(n);
println(++n);
println(n);

for (n = 1; n < 10; n*=2)
	print(n, " ");
println();

//  while Test
print("while Test\n");
a[] = { 6, 9, 12, 7, 2, 23, 10, 4 };
s = 15;
n = 0;
while ((n < array.count(a[])) && (a[n++] < s)) ;
println(n, " ", a[n]);

n = -1;
while ((n < array.count(a[])) && (a[++n] < s)) ;
println(n, " ", a[n]);

n++;
println(n, " ", a[n]);

s = 5;
n = 10;
while (n-- > s) ;
println(n);

n = 0;
while (1) {
    n = n + 1;
    print(n, " ");
    if (n % 10 == 0) print();
    if (50 < n) break;
}
print();

n = 0;
while (n < 10) {
    n = n + 1;
    print(n, " ");
}
print();
