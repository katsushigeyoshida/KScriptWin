#include "scriptLib.sc";

println("== 配列のインデックスに文字列とその入れ子 ==");
name[] = {"Orenge", "Apple" };
fruit["Orenge"] = 50;
fruit["Apple"] = 20;
a = fruit[name[0]];
println(name[0], " ", a);

fruit["Apple"] = 20;
fruit["Orenge"] = 50;
println("Orenge", " ", fruit["Orenge"], " ", "Apple", " ", fruit["Apple"]);
name[] = {"Orenge", "Apple" };
for (i = 0; i < count(name[]); i++)
	println(name[i], " = ", fruit[name[i]]);


address[ "yamada",] = { "tarou", 60, "men" };
address[ "satou",] = { "hanako", 30, "women" };

addressName = "yamada";
println(address[addressName, 0]);

a[] = { "name", 18, "men" };
println(a[1]);


a[2,] = { 100, 200, 300 };
for (i = 0; i < count(a[2,]); i++)
	print(a[2,i], ", ");
print();

person["yamada",] = { "tarou", 60, "men" };
println(person["yamada",0], ",", person["yamada",1], ",", person["yamada",2],);

person["山田", "名"] = "太郎";
person["山田", "年齢"] = 68;
person["山田", "性別"] = "男";
println(person["山田", "名"]);

item[] = { "名", "年齢", "性別" };
for (i = 0; i < count(item[]); i++) {
	print(person["山田",item[i]], " ");
}
print();


data[] = { 6, 9, 12, 7, 2, 23, 10, 4 };
index[] = { 3,2,1,0};
printArray(data[]);
a = 2;
data[a + 1] = 2;
printArray(data[]);

println("== 文字列と数値の混在配列 ==");
fruitValue[] = { "Apple", 20, "Orenge", 50 };
for (i = 0; i < count(fruitValue[]); i++) print(fruitValue[i]," ");
print();

println("== 統計計算 ==");
clear(a[]);
a[] = { 1, 2, 3, 4, 5 };
b[] = { 2, -2, 13, -4, 5 };
print("配列 a[] = "); printArray(a[]);
print("配列 b[] = "); printArray(b[]);
println("配列 a[] の数と合計 = ", count(a[]), ", ", sum(a[]));
println("配列 b[] の数と合計 = ", count(b[]), ", ", sum(b[]));
println("配列 a[] の最小最大 = ", min(a[]), ", ", max(a[]));
println("配列 b[] の最小最大 = ", min(b[]), ", ", max(b[]));
println("配列 a[] の平均 = ", average(a[]));
println("配列 b[] の平均 = ", average(b[]));
println("配列 a[] の分散 = ", variance(a[]));
println("配列 b[] の分散 = ", variance(b[]), " ", round(variance(b[])));
println("配列 a[] の標準偏差 = ", stdDeviation(a[]));
println("配列 b[] の標準偏差 = ", stdDeviation(b[]));
b[]= sort(b[]);
print("配列b[]をソート = "); printArray(b[]);
b[] = reverse(b[]);
print("配列b[]を逆順 = "); printArray(b[]);
