# 測試 HttpClient Socket 使用情況

要在 Windows 中查看當前 Socket 的使用狀況，可以使用以下命令查看端口占用情况：

```cmd
netstat -na | find "7016"
```

或開啟 PowerShell 執行以下指令來持續觀察（每秒更新一次）：

```powershell
while ($true) { cls; Write-Output "Current: $(Get-Date)"; netstat -na | Select-String "7016"; Start-Sleep -Seconds 1 }
```

## 測試

此範例測試以下使用情況：

1. 每次呼叫都建立新的 HttpClient 實體
2. 每次呼叫都透過 using 方式來建立 HttpClient 實體，使用結束後會 dispose
3. 每次呼叫都使用靜態建立的 HttpClient 實體，並重複用
4. 使用相依性注入的方式使用 HttpClient
5. 使用相依性注入的方式使用  HttpClientFactory

## netstat

`netstat` 指令可以用來查詢各種網路相關資訊，檢測各種網路相關的問題，可以列出非常多很有用的資訊，像 socket、TCP、UDP、IP 與 ethernet 層的各種資訊都可以利用 `netstat` 來查詢

此指令所輸出的 `state` 欄位，欄位指的是 TCP 連接的當前狀態，以下為常見的狀態：

- `LISTENING` 指的是伺服器上的端口正在聽取來自任何客戶端的連接請求
- `ESTABLISHED` 表示一個連接已經成功建立，數據可以在兩個方向上傳輸
- `TIME_WAIT` 本地端已經確認了遠端的終止請求，但是保持連接一段時間以確保遠端收到了最終的確認
- `CLOSED` 連接已經完全終止，不再使用

其中當 socket 處於 `TIME_WAIT` 狀態時，該 socket 的端口號是不能立即被其他新的連接使用的。`TIME_WAIT` 狀態存在的主要原因是為了確保在同一個連接的**舊**數據包在網絡中完全消失，這樣它們就不會被誤解為新連接的數據包。這是 TCP 協議的一部分，旨在確保可靠性和數據完整性。

在 TCP/IP 網絡協議中，當一個連接處於 `TIME_WAIT` 狀態時，它通常會保持這個狀態大約 2 個 Maximum Segment Lifetime（MSL）的時間，MSL 是定義 TCP/IP 網絡中任何數據包在網絡內最大生存時間的參數。

一般來說，MSL 的典型值是 2 分鐘（120 秒），所以 `TIME_WAIT` 狀態通常會持續大約 4 分鐘（240 秒）。然而，這個值可以根據操作系統和網路配置而有所不同。

## Repository

[poychang/HttpClientSocketTesting](https://github.com/poychang/HttpClientSocketTesting)
