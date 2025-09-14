text[,] = { { "abc", "cde", "def"},
      	 	{ "ABC", "CDE", "DEF"} };
path = "text.csv";
file.saveCsv(path,text[,]);
a[,] = file.loadCsv(path);
for (i = 0; i < 2; i++) {
	for (j = 0; j < 3; j++)
		print(a[i,j]," ");
	println();
}
print("ファイル選択: ");
path = file.select(".");
println(path);
text = "abcdefg\nABCD";
path = string.replace(path,".sc",".txt");
println(path);
println("ファイル名: ",file.getFileName(path));
println("ファイル名: ",file.getFileNameWithoutExtension(path));
println("拡張子: ",file.getExtention(path));
println("ディレクトリ名: ",file.getDirectory(path));
file.saveText(path,text);
path = file.select(".","テキストファイル,*.txt");
str = file.loadText(path);
println(str);
println("size:",file.size(path));
println("LastWrite: ",file.lastWrite(path,"","jp"));
println("LastWrite: ",file.lastWrite(path,"D","jp"));
	file.delete(path);
if (0 < file.fileExists(path)) {
	println("ファイルの削除:　",path);
} else {
	println("ファイルがない");
}
