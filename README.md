## สิ่งที่ต้องติดตั้ง

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Node.js และ npm](https://nodejs.org/)

## วิธี Run Backend

เปิด Terminal ที่ Root Project แล้วรันคำสั่ง

```bash
dotnet restore PipingIntegrity.sln
dotnet run --project src/backend/Piping.Api/Api --urls http://localhost:5080
```

API URL: http://localhost:5080

## วิธี Run Frontend

เปิด Terminal ใหม่ แล้วรันคำสั่ง

```bash
cd src/frontend/piping-web
npm install
npm start
```

เปิดเว็บไซต์ที่ `http://localhost:4200`

## Database

ใช้ SQLite โดยไฟล์ฐานข้อมูลอยู่ที่

```text
src/backend/Piping.Api/piping.db
```

Program.cs
setting Env ที่เครื่อง เพื่อ connect ถึง db ex. "__ที่วางโปรเจ็ค__/src/backend/Piping.Api/piping.db"

สำหรับ setting Env ที่เครื่อง
var databasePath = Path.Combine(builder.Environment.ContentRootPath, "piping.db");

สำหรับ เรียกผ่าน appsetting.json
var configuredConnection = builder.Configuration.GetConnectionString("Piping");

 ## Docker
docker load -i docker-images/piping-api.tar
docker load -i docker-images/piping-web.tar
docker compose up -d

## หมายเหตุ
- ตรวจสอบ URL ของ Backend ในการตั้งค่า Frontend
- ตั้งค่า CORS ให้รองรับ Frontend
- สำรองฐานข้อมูลก่อนแก้ไขหรือลบข้อมูลสำคัญ
