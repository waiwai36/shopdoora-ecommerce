# ShopDoora - Products & Orders Management

## Clone the Repository
```bash
git clone https://github.com/waiwai36/shopdoora-ecommerce.git
```

## Tech Stack
- **Backend:** ASP.NET Core Web API (.NET 10), Entity Framework Core, SQL Server, ASP.NET Identity, JWT Authentication
- **Frontend:** ASP.NET Core MVC (.NET 10) consuming REST API via Bearer Token

---

## Setup & Run Instructions

### 1. Database Configuration
Update the `ShopDooraDBConnection` connection string in `ShopDoora.Api/appsettings.json` to match your local SQL Server instance.

### 2. Apply Migrations
Open a terminal, navigate to the `ShopDoora.Api` directory, and run the following commands to set up the database:
```bash
cd ShopDoora.Api
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 3. Running the Project

You can run both the API and the Web frontend simultaneously using either **Visual Studio** or **VS Code**:

#### Option A: Using Visual Studio (Recommended)
1. Right-click the **Solution** in **Solution Explorer** and select **Properties**.
2. Go to **Common Properties > Startup Project** and select **Multiple startup projects**.
3. Set the **Action** to **Start** for both projects.
4. Click **OK** and press `F5` to run.
*(Quick Alternative: Run the first project normally with `F5`, then right-click the second project in Solution Explorer and select **Debug > Start New Instance**).*

#### Option B: Using VS Code
1. Press `Ctrl + Shift + P` to open the Command Palette.
2. Select **Tasks: Run Task** and choose **Run Both Projects** to start both the API and Web app at once (configured via `.vscode/tasks.json`).

---

## Application URLs
Once both projects are running, you can access them at:
- **Frontend (Web):** `http://localhost:5187/`
- **Backend (API):** `http://localhost:5000/api/`
