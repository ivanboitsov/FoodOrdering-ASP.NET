using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FoodOrderingWebsite.Migrations
{
    /// <inheritdoc />
    public partial class BaseMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            

            migrationBuilder.CreateTable(
                name: "Baskets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    DishId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Amount = table.Column<int>(type: "integer", nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    Image = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Baskets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Dishes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    Category = table.Column<string>(type: "text", nullable: false),
                    Vegetarian = table.Column<bool>(type: "boolean", nullable: false),
                    Rating = table.Column<double>(type: "double precision", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Image = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dishes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    DeliveryTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    OrderTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    Address = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Passwords",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    PasswordValue = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Passwords", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Raitings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DishId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Rating = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Raitings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TestedDishes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    DishId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestedDishes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tokens",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Token = table.Column<string>(type: "text", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsValid = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tokens", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FullName = table.Column<string>(type: "text", nullable: false),
                    Gender = table.Column<string>(type: "text", nullable: false),
                    Phone = table.Column<string>(type: "text", nullable: false),
                    Address = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    BirthDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Dishes",
                columns: new[] { "Id", "Category", "Description", "Image", "Name", "Price", "Rating", "Vegetarian" },
                values: new object[,]
                {
                    { new Guid("01b9209e-24b4-4118-b815-9723aa47403a"), "Wok", "Пшеничная лапша обжаренная на воке с фаршем (Говядина/свинина) и овощами (шампиньоны, перец сладкий, лук красный) в томатном соусе с добавлением чесночно–имбирной заправки и петрушки. БЖУ на 100 г. Белки, г — 8,07 Жиры, г — 15,38 Углеводы, г — 23,22", "https://mistertako.ru/uploads/products/663ab866-85ec-11ea-a9ab-86b1f8341741.jpg", "Wok болоньезе", 290.00m, 0.0, false },
                    { new Guid("1bf3eb95-bf69-4a1f-b74c-b8279b986ee1"), "Dessert", "Сырная лепешка, банан, арахис, сливочный сыр, шоколадная крошка, топинг карамельный", "https://mistertako.ru/uploads/products/a4772f7a-7a6f-11eb-850a-0050569dbef0.jpeg", "Сладкий ролл с арахисом и бананом", 210.00m, 0.0, true },
                    { new Guid("35170a88-0fae-4ef5-bc12-9d7854fc5477"), "Wok", "Лапша пшеничная, креветки, кальмар, шампиньоны, лук красный, заправка Том Ям (паста Том Ям, паста Том Кха, сахар, соевый соус), сливки, соевый соус, помидор, перец чили. БЖУ на 100 г. Белки, г - 8,57 Жиры, г - 12,8 Углеводы, г - 18,8", "https://mistertako.ru/uploads/products/bacd88ca-54ed-11ed-8575-0050569dbef0.jpg", "Wok том ям с морепродуктами", 340.00m, 0.0, false },
                    { new Guid("65952c14-454e-4129-8c0b-839e2bac498e"), "Pizza", "Бекон, соленый огурчик, брусника, сыр «Моцарелла», сыр «Гауда», соус BBQ", "https://mistertako.ru/uploads/products/839d0250-8327-11ec-8575-0050569dbef0.", "Party BBQ", 480.00m, 0.0, false },
                    { new Guid("6bbae1e3-659e-485d-afef-fa481619dbde"), "Drink", "Классический молочный коктейль с клубничным топпингом", "https://mistertako.ru/uploads/products/120b46bd-5f32-11e8-8f7d-00155dd9fd01.jpg", "Коктейль клубничный", 170.00m, 0.0, true },
                    { new Guid("6d1d40b1-1e3c-4751-a602-3d452b83407e"), "Dessert", "Апельсин, банан, шоколадная крошка, сыр творожный, сырная лепешка. БЖУ на 100 г. Белки, г - 5,86 Жиры, г - 13,12 Углеводы, г - 44,05", "https://mistertako.ru/uploads/products/05391255-54ee-11ed-8575-0050569dbef0.jpg", "Сладкий ролл с апельсином и бананом", 250.00m, 0.0, true },
                    { new Guid("6f18b615-aab0-4482-bd1a-08638b8c8509"), "Pizza", "4 сыра: «Моцарелла», «Гауда», «Фета», «Дор-блю», сливочно-сырный соус, пряные травы", "https://mistertako.ru/uploads/products/77888c7e-8327-11ec-8575-0050569dbef0.", "4 сыра", 360.00m, 0.0, true },
                    { new Guid("8d5b037b-014a-4eb4-8e18-20825ab85f98"), "Dessert", "Сырная лепешка, банан, киви, сливочный сыр, топинг клубничный", "https://mistertako.ru/uploads/products/9e7c8581-7a6f-11eb-850a-0050569dbef0.jpeg", "Сладкий ролл с бананом и киви", 220.00m, 0.0, true },
                    { new Guid("8dab4e30-0118-41d6-a2c3-4ab9725901f1"), "Wok", "Пшеничная лапша обжаренная на воке с колбасками пепперони, маслинами, сладким перцем и перцем халапеньо в томатном соусе с добавлением петрушки. БЖУ на 100 г. Белки, г — 8,18 Жиры, г — 16,33 Углеводы, г — 20,62", "https://mistertako.ru/uploads/products/663ab868-85ec-11ea-a9ab-86b1f8341741.jpg", "Wok а-ля Диаблo", 330.00m, 0.0, false },
                    { new Guid("af1daefb-0c03-4600-9752-5fd56e6f3ab4"), "Wok", "Лапша пшеничная, куриное филе, шампиньоны, лук красный, заправка Том Ям (паста Том Ям, паста Том Кха, сахар, соевый соус), сливки, соевый соус, помидор, перец чили. БЖУ на 100 г. Белки, г - 7,05 Жиры, г - 12,92 Углеводы, г - 18,65", "https://mistertako.ru/uploads/products/a41bd9fd-54ed-11ed-8575-0050569dbef0.jpg", "Wok том ям с курицей", 280.00m, 0.0, false },
                    { new Guid("b6fa87bd-b8eb-4f33-9ded-b1bc66dea8b5"), "Wok", "Лапша пшеничная, шампиньоны, лук красный, заправка Том Ям (паста Том Ям, паста Том Кха, сахар, соевый соус), сливки, соевый соус, помидор, перец чили. БЖУ на 100 г. Белки, г - 5,32 Жиры, г - 14,89 Углеводы, г - 22,46", "https://mistertako.ru/uploads/products/cd661716-54ed-11ed-8575-0050569dbef0.jpg", "Wok том ям с овощами", 250.00m, 0.0, true },
                    { new Guid("cf26dec6-ad06-4ba7-b671-9e0e12028788"), "Soup", "Сырный бульон с пшеничной лапшой, отварным куриным филе, помидором и сырными шариками. БЖУ на 100 г. Белки, г — 11,8 Жиры, г — 9,82 Углеводы, г — 22,69", "https://mistertako.ru/uploads/products/ccd8e2de-5f36-11e8-8f7d-00155dd9fd01.jpg", "Рамен сырный", 300.00m, 0.0, false },
                    { new Guid("d33d79ab-4886-4fa8-8f3e-cff1f0a2e1e4"), "Drink", "Смородиновый морс", "https://mistertako.ru/uploads/products/120b46c1-5f32-11e8-8f7d-00155dd9fd01.jpg", "Морс cмородиновый", 90.00m, 0.0, true },
                    { new Guid("d4d0c0a8-2293-484a-b129-8c759a896446"), "Soup", "Бульон рамен со сливками (куриный бульон, чесночно-имбирная заправка, соевый соус) с пшеничной лапшой, отварной курицей, омлетом Томаго и шампиньонами. БЖУ на 100 г. Белки, г — 8,13 Жиры, г — 6,18 Углеводы, г — 8,08", "https://mistertako.ru/uploads/products/ccd8e2de-5f36-11e8-8f7d-00155dd9fd01.jpg", "Сливочный рамен с курицей и шампиньонами", 260.00m, 0.0, false },
                    { new Guid("d795997a-4923-44ae-a2a5-609cab54c9d1"), "Dessert", "Чизкейк Нью-Йорк - настоящая классика. Его основа - сочетание вкусов нежнейшего сливочного сыра и тонкой песочной корочки.", "https://mistertako.ru/uploads/products/120b46b1-5f32-11e8-8f7d-00155dd9fd01.jpg", "Чизкейк Нью-Йорк", 210.00m, 0.0, true },
                    { new Guid("d978c82f-49d1-4727-b4a7-4521353af33f"), "Drink", "Классический молочный коктейль с добавлением шоколадного топпинга", "https://mistertako.ru/uploads/products/120b46be-5f32-11e8-8f7d-00155dd9fd01.jpg", "Коктейль шоколадный", 170.00m, 0.0, true },
                    { new Guid("e1a09a59-1a0c-4fe8-a2c7-08b058b9106b"), "Soup", "Знаменитый тайский острый суп со сливками, куриным филе, шампиньонами, красным луком, помидором, перчиком Чили и кинзой. Подается с рисом. БЖУ на 100 г. Белки, г — 5,75 Жиры, г — 3,72 Углеводы, г — 14,76", "https://mistertako.ru/uploads/products/ccd8e2de-5f36-11e8-8f7d-00155dd9fd01.jpg", "Том ям кай", 300.00m, 0.0, false },
                    { new Guid("e5160ba6-c051-421c-be2e-d5ed470630fb"), "Pizza", "Копченая куриная грудка, свежие шампиньоны, маринованные опята, сыр «Моцарелла», сыр «Гауда», сливочно-чесночный соус, свежая зелень.", "https://mistertako.ru/uploads/products/9ee8ed49-8327-11ec-8575-0050569dbef0.", "Белиссимо", 400.00m, 0.0, false },
                    { new Guid("fa56a0c6-6763-422d-8996-d78e21fe1abb"), "Drink", "Классический молочный коктейль", "https://mistertako.ru/uploads/products/120b46bc-5f32-11e8-8f7d-00155dd9fd01.jpg", "Коктейль классический", 140.00m, 0.0, true },
                    { new Guid("fbce39a3-f2f9-4f12-ba05-2531474882cb"), "Drink", "Облепиха, имбирь, сахар", "https://mistertako.ru/uploads/products/5a7d58a5-879d-11eb-850a-0050569dbef0.jpg", "Морс облепиховый", 90.00m, 0.0, true }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropTable(
                name: "Baskets");

            migrationBuilder.DropTable(
                name: "Dishes");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Passwords");

            migrationBuilder.DropTable(
                name: "Raitings");

            migrationBuilder.DropTable(
                name: "TestedDishes");

            migrationBuilder.DropTable(
                name: "Tokens");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
