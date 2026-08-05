//	気象庁の気候データからダウンロード(羽田は1993年以前のデータなし)
//	https://www.data.jma.go.jp/risk/obsdl/index.php
//
//folder = "C:\\Users\\k-yos\\OneDrive\\一時データ\\オープンデータ\\気象データ\\";
folder = file.getDirectory(file.getScriptPath()) + "\\気象データ\\";
println(folder);

//
file2026 = "data_気温_羽田_2026.csv";
file2025 = "data_気温_羽田_2025.csv";
file2024 = "data_気温_羽田_2024.csv";
file2023 = "data_気温_羽田_2023.csv";
file2022 = "data_気温_羽田_2022csv";
file2021 = "data_気温_羽田_2021.csv";
file2020 = "data_気温_羽田_2020.csv";
file2019 = "data_気温_羽田_2019.csv";
file2018 = "data_気温_羽田_2018.csv";
file2017 = "data_気温_羽田_2017.csv";
file2016 = "data_気温_羽田_2016.csv";
札幌2026 = "data_気温_札幌_2026.csv";
札幌2025 = "data_気温_札幌_2025.csv";
札幌2024 = "data_気温_札幌_2024.csv";
札幌2023 = "data_気温_札幌_2023.csv";
札幌2022 = "data_気温_札幌_2022.csv";
札幌2021 = "data_気温_札幌_2021.csv";

splitArea = 4;
wetheGraph(folder,札幌2026,splitArea,0,1,2);
wetheGraph(folder,file2026,splitArea,1,1,2);
wetheGraph(folder,札幌2025,splitArea,2,1,2);
wetheGraph(folder,file2025,splitArea,3,1,2);
//wetheGraph(folder,札幌2022,splitArea,4,1,2);
//wetheGraph(folder,file2022,splitArea,5,1,3);
//wetheGraph(folder,file2015,4,1,3);
//wetheGraph(folder,file2000,4,1,3);

wetheGraph(folder,file,splitArea,useArea,high,low) {
	//	ファイルからデータの読込
	path = folder + file;
	println(path);
	file.setEncordingType("ShiftJis");
	array.clear(data[,]);
	data[,] = file.loadCsv(path);
	// データからタイトル行削除
	for (i = 0; i < 4; i++)
		array.remove(data[i,]);
	array.squeeze(data[,]);
//	file.saveCsv(file, data[,]);
	// グラフ表示
	splitAreaX = 1;
	splitAreaY = splitArea;
	graph.SetSplitArea(splitAreaX,splitAreaY);
	graph.SetUseArea(useArea);
	graph.Title(file);
	graph.YTitle("気温");
	graph.XTitle("日付");
	graph.GraphType("line");
	graph.SetDataArea(0,-20,366,40);
	//	最高気温
	graph.SetColor("Red");
	graph.SetData(data[,0],data[,high]);
	//	最低気温
	graph.SetColor("Blue");
	graph.AddData(data[,0],data[,low]);
}