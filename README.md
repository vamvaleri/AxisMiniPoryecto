# ✦ Axis — Gestión de Ventas e Inventario

Sistema de punto de venta y control de stock desarrollado en **.NET 8 (C#)** con arquitectura compartida entre Web y Escritorio:
- **Biblioteca Compartida (`Papeleria.Core`)**: Entidades (`Producto`, `Cliente`, `Venta`, `DetalleVenta`), Herencia/Polimorfismo (`ProductoEscolar`), Estructuras de Datos (`List<T>`, `Dictionary<K,V>`, `Queue<T>`) y **Entity Framework Core (SQLite)**.
- **Aplicación Web Responsive (`Papeleria.Web`)**: ASP.NET Core MVC, Bootstrap 5 y ASP.NET Core Identity con 2 roles (`Administrador` y `Vendedor`).
- **Aplicación de Escritorio Gráfica (`Papeleria.Desktop`)**: Interfaz gráfica de Windows Forms con componentes personalizados (`BotonAxis`, `PanelBento`) y gráficos vectoriales GDI+.

## Ejecución
1. Abre una terminal dentro de la carpeta raíz (`PapeleriaVentasInventario`).
2. Para ejecutar **Axis Escritorio (Windows Forms)**:
   ```bash
   dotnet run --project Papeleria.Desktop/Papeleria.Desktop.csproj
   ```
3. Para ejecutar **Axis Web (ASP.NET Core MVC)**:
   ```bash
   dotnet run --project Papeleria.Web/Papeleria.Web.csproj
   ```
   - **Administrador:** `admin@papeleria.com` / Clave: `1234`
   - **Vendedor:** `vendedor@papeleria.com` / Clave: `1234`
