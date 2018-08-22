BingMap Viewer
https://msdn.microsoft.com/en-us/library/bb259689.aspx  bingmap 打格子方式
a r h 三種圖層，第一層為  a0 a1 a2 a3 四格， 每格256*256，四格也就是也就是 512*512
https://ecn.t1.tiles.virtualearth.net/tiles/a0.jpeg?g=3649   
https://ecn.t1.tiles.virtualearth.net/tiles/r0.jpeg?g=3649
https://ecn.t1.tiles.virtualearth.net/tiles/h0.jpeg?g=3649
https://t1.ssl.ak.dynamic.tiles.virtualearth.net/comp/ch/0?mkt=zh-TW&it=G,L&shading=hill&n=z&cstl=rd&og=142&c4w=1

https://maps.googleapis.com/maps/api/geocode/xml?address=%E5%B0%8F%E6%B8%AF%E6%A9%9F%E5%A0%B4

功能:
滾輪放大、滾輪縮小、按鈕放大、按鈕縮小、按鈕縮放至全圖、框選縮放至最接近可是範圍、滑鼠左鍵平移、
切換圖層、狀態列顯示滑鼠經緯度、關鍵字定位、建立帳號(SQL SERVER)、登入介面、登入後重返上次視野與設定、
預覽列印、列印

內建圖層:
讀檔: 線圖層，高雄捷運路線圖(紅、橘線圖層)-Khsc_mrt.geo、Khsc_mrt.csv
讀檔: 面圖層，高雄市行政區界，半透明藍色面圖層，區名寫在外接矩形的正中間-Khsc_town.geo、Khsc_town.csv
資料庫圖層:點圖層，每次viewport變更都會查詢資料庫取得範圍內的地標，以符號作畫，滑鼠移至符號上，以tooltip顯示地標資訊
(只實作停車場、加油站、捷運站、學校)-Khsc_landmark.geo、Khsc_landmark.csv

類別
MapView，使用者控制項，取得Graphics 作畫
Layer，圖層父類別，底下有BingMapsLayer、MrtLayer、TownLayer、LandmarkLayer

**環境，.NET 4.0 C# windows form，不可使用第三方元件


22.5746290,120.3448068