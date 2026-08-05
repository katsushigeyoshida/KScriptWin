//	気象庁の気候データからダウンロード(羽田は1993年以前のデータなし)
//	https://www.data.jma.go.jp/risk/obsdl/index.php
//
//folder = "C:\\Users\\k-yos\\OneDrive\\一時データ\\オープンデータ\\気象データ\\";
folder = file.getDirectory(file.getScriptPath()) + "\\気象データ\\";
file2026 = "data_気温_羽田_2026.csv";
file2025 = "data_気温_羽田_2025.csv";
file2024 = "data_気温_羽田_2024.csv";
file2023 = "data_気温_羽田_2023.csv";
file2022 = "data_気温_羽田_2022.csv";
file2021 = "data_気温_羽田_2021.csv";
file2020 = "data_気温_羽田_2020.csv";
file2019 = "data_気温_羽田_2019.csv";
file2018 = "data_気温_羽田_2018.csv";
file2017 = "data_気温_羽田_2017.csv";
file2016 = "data_気温_羽田_2016.csv";


//
file[] = { 
file2026, 
file2025, file2024, file2023, file2022, file2021, 
file2020, file2019, file2018, file2017, file2016,
};

start = 0;			//	表示開始データ
splitAreaX = 1;
splitAreaY = 5;
graph.SetSplitArea(splitAreaX,splitAreaY);
for (i = start; i < start + splitAreaX*splitAreaY; i++) {
	data[,] = loadData(folder, file[i]);
	//	グラフ表示
	graph.SetUseArea((i - start) % splitArea);
	graph.Title(file[i]);
	graph.YTitle("気温");
	graph.XTitle("日付");
	graph.SetDataArea(0,-10,366,40);
	graph.GraphType("line");
	graph.SetColor("Red");
	graph.SetData(data[,0],data[,1]);
	graph.SetColor("Blue");
	graph.AddData(data[,0],data[,2]);
}


loadData(folder,file) {
startTime();
	path = folder + file;
	println(path);
	file.setEncordingType("ShiftJis");
	array.clear(data[,]);
	data[,] = file.loadCsv(path);
println(lapTime());
	//	タイトル行削除
	for (i = 0; i < 4; i++)
		array.remove(data[i,]);
println(lapTime());
	array.squeeze(data[,]);
println(lapTime());
//	for (i = 0; i < 10;i++)
//		println(data[i,]);
//	file.saveCsv(file, data[,]);
	return data[,];
}
