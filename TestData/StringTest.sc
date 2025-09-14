str1 = "abcd";
str2 = "def";
str = string.concat(str1,str2,"ABCD");
println("文字列の接続: ",str," ");
str = str1 +str2;
println("文字列の接続: ",str," ");
println("文字の長さ: ",string.length(str));

a = string.substring(str,2,3);
println("部分取得: ",a);

arr[] = { "abc", "cde", "efgg" };
str = string.join(",",arr[]);
println("配列の文字列の連結: ",str);
arr2[] = string.split(str,",");
print("文字列の分割: ");
for (i = 0; i < array.count(arr2[]); i++)
	print(arr2[i]," ");
println();

println("文字を含むかの判定: ",string.contains(str,",cd"));
println("文字の位置: ",string.indexOf(str,"c",3));
println("文字の置換: ",string.replace(str,",","&"));
println("文字の挿入: ",string.insert(str,2,"AB"));
println("文字の削除: ",string.remove(str,2,2));

b = 123456;
println(string.format("{0:G}",b));
c = 0.02123;
println(string.format("{0:P3}",c));
println("toString   : ", string.toString(b));
println("toString(C): ", string.toString("C",b));
println("toString   : ", string.toString(c));
println("toString(E): ", string.toString("E",c));
println("toString(P): ", string.toString("P3",c));
d = 28;
println("toString(X): ", string.toString("X",d));
println("toString(X): ", string.toString("15:D",d));
println("padLeft: ",string.padLeft("AB",10,"-"));
println("padLeft: ",string.padLeft("ABCD",10,"-"));
println("padRight: ",string.padRight("AB",10,"-"));
println("padRight: ",string.padRight("ABCD",10,"-"));
