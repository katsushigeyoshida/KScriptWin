str1 = "abcd";
str2 = "def";
str = string.concat(str1,str2,"ABCD");
println("文字列の接続: ",str," ");
str = str1 +str2;
println("文字の長さ: ",str," → ",string.length(str));
a = string.substring(str,2,3);
println("部分取得: ",str," → ",a);

arr[] = { "abc", "cde", "efgg" };
str = string.join(",",arr[]);
println("配列の文字列の連結: ",str);
arr2[] = string.split(str,",");
print("文字列の分割: ");
for (i = 0; i < array.count(arr2[]); i++)
	print(arr2[i]," ");
println();

println("文字を含むかの判定: ",str," → ",string.contains(str,",cd"));
println("文字比較: ",string.compare("ad","cd")," ",string.compare("ad","ad"));
println("文字の位置: ",str," → ",string.indexOf(str,"c",3));
println("文字の置換: ",str," → ",string.replace(str,",","&"));
println("文字の挿入: ",str," → ",string.insert(str,2,"AB"));
println("文字の削除: ",str," → ",string.remove(str,2,2));

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
println("padRight(str): ",str," → ",string.padRight("AB",10,"-"));
println("padRight(str): ",str," → ",string.padRight("ABCD",10,"-"));
println("toUpper(str): ",str," → ",string.toUpper(str));
str = string.toUpper(str);
println("toLower(str): ",str," → ",string.toLower(str));
str = "  abcd  ";
println("trim(str): [",str,"] → [",string.trim(str),"]");