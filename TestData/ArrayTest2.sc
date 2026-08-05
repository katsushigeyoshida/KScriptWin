//	配列関数のテスト

println("配列のコピー操作");
p[,] = {{1,2,3,4},{4,5,6,7},{7,8,9,10}};
println(p[,]);
println(p[2,]);
println(p[,2]);
a[,] = {{0,1,2},{2,4,5}};
plist[0,] = p[a[1,0],];
println("plist[0,i] : ", plist[0,]);

println("配列の大きさ count(plist[,]) count(p[,]) count(p[,0]) count(p[0,])");
println("p[,] count: ",array.count(plist[,])," ",array.count(p[,])," ",array.count(p[,0])," ",array.count(p[0,]));

println("配列の初期化");
w = 10;
d = 8;
t = 0.5;
v[] = { -w/2,0,(-d+t)/2};
println("配列の大きさ(count) v[]: ",array.count(v[]));
println("v[] : ",v[]);

println("配列に値の追加 append(v[], -10)");
array.append(v[], -10);
println("v[] : ", v[]);

println("配列に値の加算(add)");
for (i = 0; i < array.count(plist[,0]); i++) {
	println(plist[i,]);
}
println("2を加算 add(plist[,],2)");
array.add(plist[,],2);
for (i = 0; i < array.count(plist[,0]); i++) {
	println(plist[i,]);
}
println();

println("間隔のの数値配列作成 arrange(1, 10, 2)");
array.clear(a[,]);
a[] = array.arrange(1,10,2);
println("a[] : ",a[]);

println("等分割した数値配列の作成 linspace(0, 10, 5)");
array.clear(a[,]);
a[] = array.linspace(0, 10, 5);
println("a[] linspace : ", a[]);
println();

println("配列の作成");
array.clear(a[]);
array.clear(a[,]);
array.clear(a[,,]);
println("create(10,2) : ");
a[] = array.create(10,2);
println(a[]);

println("create(3,5,5) : ");
a[,] = array.create(3,5,5);
println(a[,]);

println("create(3,4,5,6) : ");
a[,,] = array.create(3,4,5,6);
println(a[,,]);
