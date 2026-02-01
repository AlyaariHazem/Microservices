# 📚 توثيق نظام الخدمات المصغرة (Microservices)

**التاريخ:** يناير 2026  
**المطور:** حازم الياعري  
**الحالة:** قيد التطوير المستمر

---

## 📑 فهرس المحتويات

1. [نظرة عامة على النظام](#1-نظرة-عامة-على-النظام)
2. [البنية المعمارية](#2-البنية-المعمارية)
3. [التقنيات المستخدمة](#3-التقنيات-المستخدمة)
4. [الخدمات](#4-الخدمات)
   - [خدمة الكتالوج (Catalog Service)](#41-خدمة-الكتالوج-catalog-service)
   - [خدمة السلة (Basket Service)](#42-خدمة-السلة-basket-service)
   - [خدمة الخصومات (Discount Service)](#43-خدمة-الخصومات-discount-service)
   - [خدمة الطلبات (Ordering Service)](#44-خدمة-الطلبات-ordering-service)
5. [المكتبات المشتركة (BuildingBlocks)](#5-المكتبات-المشتركة-buildingblocks)
6. [قواعد البيانات](#6-قواعد-البيانات)
7. [واجهات البرمجة (APIs)](#7-واجهات-البرمجة-apis)
8. [تشغيل النظام](#8-تشغيل-النظام)
9. [أنماط التصميم المستخدمة](#9-أنماط-التصميم-المستخدمة)
10. [التطويرات المستقبلية](#10-التطويرات-المستقبلية)

---

## 1. نظرة عامة على النظام

### 1.1 الوصف

هذا المشروع هو تطبيق عملي لبنية **الخدمات المصغرة (Microservices)** باستخدام **.NET 8** وأحدث الممارسات في تطوير البرمجيات السحابية. يهدف المشروع إلى بناء نظام تجارة إلكترونية متكامل يتكون من خدمات مستقلة تتواصل فيما بينها.

### 1.2 الأهداف

- ✅ بناء خدمات مصغرة مستقلة وقابلة للتوسع
- ✅ تطبيق أنماط التصميم الحديثة (CQRS, Clean Architecture, DDD)
- ✅ استخدام تقنيات الاتصال المتنوعة (REST, gRPC)
- ✅ تطبيق مبدأ "قاعدة بيانات لكل خدمة"
- ✅ دعم الحاويات باستخدام Docker

### 1.3 المميزات الرئيسية

| الميزة | الوصف |
|--------|-------|
| خدمات مستقلة | كل خدمة يمكن نشرها وتوسيعها بشكل منفصل |
| أنماط اتصال متعددة | REST APIs و gRPC |
| قاعدة بيانات لكل خدمة | PostgreSQL, Redis, SQLite |
| الحاويات | دعم كامل لـ Docker و Docker Compose |

---

## 2. البنية المعمارية

### 2.1 رسم تخطيطي للنظام

```
┌─────────────────────────────────────────────────────────────────────┐
│                         طبقة العملاء                                │
│                    ┌─────────────────────┐                          │
│                    │  تطبيقات العملاء    │                          │
│                    └─────────┬───────────┘                          │
└──────────────────────────────┼──────────────────────────────────────┘
                               │
┌──────────────────────────────┼──────────────────────────────────────┐
│                         طبقة الخدمات                                │
│      ┌───────────────┬───────┴───────┬───────────────┐              │
│      │               │               │               │              │
│      ▼               ▼               ▼               ▼              │
│ ┌─────────┐    ┌─────────┐    ┌─────────┐    ┌─────────────┐        │
│ │ Catalog │    │ Basket  │    │Ordering │    │  Discount   │        │
│ │   API   │    │   API   │    │   API   │    │   (gRPC)    │        │
│ │ REST+   │    │ REST+   │    │  REST   │    │             │        │
│ │ Carter  │    │ Carter  │    │ MediatR │    │             │        │
│ └────┬────┘    └────┬────┘    └────┬────┘    └──────┬──────┘        │
│      │              │              │                │               │
└──────┼──────────────┼──────────────┼────────────────┼───────────────┘
       │              │              │                │
┌──────┼──────────────┼──────────────┼────────────────┼───────────────┐
│      │         طبقة البيانات       │                │               │
│      ▼              ▼              ▼                ▼               │
│ ┌─────────┐  ┌───────────────┐ ┌─────────┐    ┌─────────┐           │
│ │PostgreSQL│ │ PostgreSQL    │ │SQL Server│   │ SQLite  │           │
│ │CatalogDB │ │ BasketDB      │ │OrderDB   │   │DiscountDB│          │
│ └─────────┘  │ + Redis Cache │ └─────────┘    └─────────┘           │
│              └───────────────┘                                       │
└─────────────────────────────────────────────────────────────────────┘
```

### 2.2 مبادئ البنية

1. **الاستقلالية**: كل خدمة تعمل بشكل مستقل
2. **قاعدة بيانات منفصلة**: كل خدمة لها قاعدة بياناتها الخاصة
3. **الاتصال عبر APIs**: REST للعملاء، gRPC للاتصال بين الخدمات
4. **الحاويات**: جميع الخدمات معبأة في حاويات Docker

### 2.3 أنماط الاتصال

| النوع | الاستخدام | المثال |
|-------|----------|--------|
| **REST** | اتصال العملاء بالخدمات | Catalog API, Basket API |
| **gRPC** | اتصال الخدمات ببعضها | Basket ← Discount |

---

## 3. التقنيات المستخدمة

### 3.1 إطار العمل الأساسي

| التقنية | الإصدار | الاستخدام |
|---------|---------|----------|
| **.NET** | 8.0 | إطار العمل الأساسي |
| **ASP.NET Core** | 8.0 | بناء APIs |
| **Carter** | - | توجيه Minimal APIs |
| **MediatR** | - | تطبيق نمط CQRS |
| **FluentValidation** | - | التحقق من المدخلات |
| **Mapster** | - | تحويل الكائنات |

### 3.2 قواعد البيانات

| قاعدة البيانات | الاستخدام |
|---------------|----------|
| **PostgreSQL** | الكتالوج والسلة |
| **Marten** | مستندات وأحداث على PostgreSQL |
| **Redis** | التخزين المؤقت الموزع |
| **SQLite** | خدمة الخصومات |
| **SQL Server** | خدمة الطلبات |

### 3.3 الاتصالات

| التقنية | الاستخدام |
|---------|----------|
| **gRPC** | اتصال عالي الأداء بين الخدمات |
| **REST** | APIs قياسية عبر HTTP |
| **Protobuf** | تسلسل البيانات لـ gRPC |

### 3.4 البنية التحتية

| التقنية | الاستخدام |
|---------|----------|
| **Docker** | حاويات التطبيق |
| **Docker Compose** | تنسيق الحاويات المتعددة |

---

## 4. الخدمات

### 4.1 خدمة الكتالوج (Catalog Service)

#### الوصف
خدمة إدارة المنتجات والكتالوج، تتيح إضافة وتعديل وحذف واستعراض المنتجات.

#### التقنيات
- **Carter** للتوجيه
- **Marten** لقاعدة البيانات المستندية
- **PostgreSQL** للتخزين
- **MediatR** لنمط CQRS
- **FluentValidation** للتحقق

#### المنافذ
| المنفذ | البروتوكول |
|-------|-----------|
| 5000 | HTTP |
| 5050 | HTTPS |

#### نموذج البيانات

```
المنتج (Product)
├── Id (UUID) - المعرف الفريد
├── Name (string) - اسم المنتج
├── Category (list) - التصنيفات
├── Description (string) - الوصف
├── ImageFile (string) - صورة المنتج
└── Price (decimal) - السعر
```

#### نقاط النهاية (Endpoints)

| الطريقة | المسار | الوصف |
|---------|--------|-------|
| `GET` | `/products` | عرض جميع المنتجات |
| `GET` | `/products/{id}` | عرض منتج محدد |
| `GET` | `/products/category/{category}` | عرض منتجات تصنيف معين |
| `POST` | `/products` | إضافة منتج جديد |
| `PUT` | `/products` | تعديل منتج |
| `DELETE` | `/products/{id}` | حذف منتج |

#### مثال على الاستخدام

**إضافة منتج جديد:**
```json
POST /products
Content-Type: application/json

{
  "name": "iPhone 14",
  "category": ["Electronics", "Smartphones"],
  "description": "أحدث هاتف من آبل",
  "imageFile": "iphone14.png",
  "price": 999.99
}
```

---

### 4.2 خدمة السلة (Basket Service)

#### الوصف
خدمة إدارة سلة التسوق، تتيح للمستخدمين إضافة المنتجات للسلة وتطبيق الخصومات.

#### التقنيات
- **Carter** للتوجيه
- **PostgreSQL** للتخزين
- **Redis** للتخزين المؤقت
- **gRPC Client** للاتصال بخدمة الخصومات
- **MediatR** لنمط CQRS

#### المنافذ
| المنفذ | البروتوكول |
|-------|-----------|
| 6001 | HTTP |
| 6061 | HTTPS |

#### نموذج البيانات

```
سلة التسوق (ShoppingCart)
├── UserName (string) - اسم المستخدم
├── Items (list) - عناصر السلة
│   ├── Quantity (int) - الكمية
│   ├── Color (string) - اللون
│   ├── Price (decimal) - السعر
│   ├── ProductId (UUID) - معرف المنتج
│   └── ProductName (string) - اسم المنتج
└── TotalPrice (decimal) - السعر الإجمالي
```

#### نقاط النهاية (Endpoints)

| الطريقة | المسار | الوصف |
|---------|--------|-------|
| `GET` | `/basket/{username}` | عرض سلة المستخدم |
| `POST` | `/basket` | إنشاء/تحديث السلة |
| `DELETE` | `/basket/{username}` | حذف السلة |
| `POST` | `/basket/checkout` | إتمام الشراء |

#### التكاملات
- تستدعي **خدمة الخصومات** عبر gRPC للحصول على الخصومات

#### مثال على الاستخدام

**إنشاء سلة:**
```json
POST /basket
Content-Type: application/json

{
  "userName": "hazem_user",
  "items": [
    {
      "quantity": 2,
      "color": "أسود",
      "price": 999.99,
      "productId": "5334c996-8457-4cf0-815c-ed2b77c4ff61",
      "productName": "iPhone 14"
    }
  ]
}
```

---

### 4.3 خدمة الخصومات (Discount Service)

#### الوصف
خدمة إدارة الخصومات والكوبونات، توفر معلومات الخصومات للخدمات الأخرى عبر بروتوكول gRPC.

#### التقنيات
- **gRPC Server** للاتصال
- **SQLite** للتخزين خفيف الوزن
- **Entity Framework Core** للوصول للبيانات
- **Protobuf** لتعريف العقود

#### المنافذ
| المنفذ | البروتوكول |
|-------|-----------|
| 6002 | HTTP/gRPC |
| 6062 | HTTPS/gRPC |

#### نموذج البيانات

```
الكوبون (Coupon)
├── Id (int) - المعرف
├── ProductName (string) - اسم المنتج
├── Description (string) - الوصف
└── Amount (int) - قيمة الخصم
```

#### طرق gRPC

| الطريقة | الوصف |
|---------|-------|
| `GetAllDiscounts()` | عرض جميع الخصومات |
| `GetDiscount(productName)` | عرض خصم منتج معين |
| `CreateDiscount(coupon)` | إنشاء خصم جديد |
| `UpdateDiscount(coupon)` | تحديث خصم |
| `DeleteDiscount(productName)` | حذف خصم |

---

### 4.4 خدمة الطلبات (Ordering Service)

#### الوصف
خدمة معالجة الطلبات باستخدام مبادئ **Clean Architecture** و **Domain-Driven Design (DDD)**.

#### التقنيات
- **Clean Architecture** (الهندسة النظيفة)
- **MediatR** لنمط CQRS
- **Entity Framework Core**
- **Domain-Driven Design** (التصميم المبني على النطاق)

#### المنافذ (مخطط)
| المنفذ | البروتوكول |
|-------|-----------|
| 7000 | HTTP |
| 7050 | HTTPS |

#### طبقات الخدمة

```
Ordering Service
├── Ordering.API           → نقاط النهاية والمتحكمات
├── Ordering.Application   → حالات الاستخدام (CQRS)
├── Ordering.Domain        → نماذج النطاق والتجميعات
└── Ordering.Infrastructure → الوصول للبيانات
```

#### نموذج النطاق (Domain Model)

```
الطلب (Order) - جذر التجميع
├── OrderId - معرف الطلب
├── CustomerId - معرف العميل
├── OrderDate - تاريخ الطلب
├── Status - حالة الطلب
├── Items - عناصر الطلب
│   ├── ProductId - معرف المنتج
│   ├── ProductName - اسم المنتج
│   ├── Quantity - الكمية
│   ├── UnitPrice - سعر الوحدة
│   └── TotalPrice - السعر الإجمالي
└── TotalAmount - المبلغ الإجمالي

كائنات القيمة (Value Objects)
├── Address - العنوان
├── Payment - الدفع
└── OrderName - اسم الطلب
```

> ⚠️ **ملاحظة**: هذه الخدمة قيد التطوير، البنية جاهزة لكن نقاط النهاية لم تُفعَّل بعد.

---

## 5. المكتبات المشتركة (BuildingBlocks)

### الوصف
مكتبة مشتركة تحتوي على المكونات القابلة لإعادة الاستخدام بين الخدمات.

### المكونات

| المكون | الوصف |
|--------|-------|
| **ICommand / IQuery** | واجهات CQRS |
| **Pipeline Behaviors** | سلوكيات MediatR |
| **Validation Behaviors** | التحقق التلقائي |
| **Custom Exceptions** | استثناءات مخصصة |

### هيكل المكتبة

```
BuildingBlocks/
├── CQRS/
│   ├── ICommand.cs
│   ├── ICommandHandler.cs
│   ├── IQuery.cs
│   └── IQueryHandler.cs
├── Behaviors/
│   ├── ValidationBehavior.cs
│   └── LoggingBehavior.cs
└── Exceptions/
    ├── NotFoundException.cs
    └── BadRequestException.cs
```

---

## 6. قواعد البيانات

### 6.1 جدول ملخص

| الخدمة | قاعدة البيانات | المنفذ | الاستخدام |
|--------|--------------|-------|----------|
| Catalog | PostgreSQL | 5432 | تخزين المنتجات |
| Basket | PostgreSQL | 5433 | تخزين السلات |
| Basket | Redis | 6379 | التخزين المؤقت |
| Discount | SQLite | - | تخزين الخصومات |
| Ordering | SQL Server | - | تخزين الطلبات |

### 6.2 بيانات الاتصال

#### PostgreSQL - Catalog
```
Host: localhost
Port: 5432
Database: CatalogDb
User: postgres
Password: postgres
```

#### PostgreSQL - Basket
```
Host: localhost
Port: 5433
Database: BasketDb
User: postgres
Password: postgres
```

#### Redis
```
Host: localhost
Port: 6379
```

---

## 7. واجهات البرمجة (APIs)

### 7.1 الوصول لـ Swagger

| الخدمة | الرابط |
|--------|-------|
| Catalog API | http://localhost:5000/swagger |
| Basket API | http://localhost:6001/swagger |

### 7.2 ملخص جميع نقاط النهاية

#### Catalog API
```http
GET    /products                    # عرض جميع المنتجات
GET    /products/{id}               # عرض منتج محدد
GET    /products/category/{category} # منتجات تصنيف معين
POST   /products                    # إضافة منتج
PUT    /products                    # تعديل منتج
DELETE /products/{id}               # حذف منتج
```

#### Basket API
```http
GET    /basket/{username}           # عرض سلة المستخدم
POST   /basket                      # إنشاء/تحديث السلة
DELETE /basket/{username}           # حذف السلة
POST   /basket/checkout             # إتمام الشراء
```

#### Discount gRPC
```protobuf
service DiscountProtoService {
  rpc GetAllDiscounts (GetAllDiscountsRequest) returns (CouponsModel);
  rpc GetDiscount (GetDiscountRequest) returns (CouponModel);
  rpc CreateDiscount (CreateDiscountRequest) returns (CouponModel);
  rpc UpdateDiscount (UpdateDiscountRequest) returns (CouponModel);
  rpc DeleteDiscount (DeleteDiscountRequest) returns (DeleteDiscountResponse);
}
```

---

## 8. تشغيل النظام

### 8.1 المتطلبات الأساسية

| البرنامج | الإصدار | الرابط |
|---------|---------|-------|
| .NET SDK | 8.0+ | [تحميل](https://dotnet.microsoft.com/download/dotnet/8.0) |
| Docker Desktop | أحدث إصدار | [تحميل](https://www.docker.com/products/docker-desktop) |
| Git | أحدث إصدار | [تحميل](https://git-scm.com/) |

### 8.2 التشغيل باستخدام Docker (موصى به)

```bash
# 1. استنساخ المستودع
git clone https://github.com/AlyaariHazem/Microservices.git
cd Microservices

# 2. بناء وتشغيل جميع الحاويات
docker-compose up -d

# 3. عرض السجلات
docker-compose logs -f

# 4. إيقاف الحاويات
docker-compose down
```

### 8.3 التشغيل اليدوي

```bash
# تشغيل Catalog API
cd src/Services/Catalog/Catalog.API
dotnet run

# تشغيل Basket API (في نافذة جديدة)
cd Basket.API
dotnet run

# تشغيل Discount gRPC (في نافذة جديدة)
cd src/Services/Discount/Discount.Grpc
dotnet run
```

### 8.4 حاويات Docker

| الخدمة | الحاوية | المنافذ |
|--------|---------|--------|
| catalog.api | catalogapi | 5000, 5050 |
| basket.api | basketapi | 6001, 6061 |
| discount.grpc | discountgrpc | 6002, 6062 |
| catalogdb | catalogdb | 5432 |
| basketdb | basketdb | 5433 |
| distributedcache | distributedcache | 6379 |

---

## 9. أنماط التصميم المستخدمة

### 9.1 CQRS (فصل مسؤولية الأوامر والاستعلامات)

```
CQRS Pattern
├── Commands (الأوامر)
│   ├── CreateProductCommand
│   ├── UpdateProductCommand
│   └── DeleteProductCommand
└── Queries (الاستعلامات)
    ├── GetProductsQuery
    └── GetProductByIdQuery
```

**الفوائد:**
- فصل عمليات القراءة عن الكتابة
- تحسين الأداء
- سهولة التوسع

### 9.2 Repository Pattern (نمط المستودع)

**الفوائد:**
- عزل طبقة الوصول للبيانات
- سهولة الاختبار
- مرونة تغيير قاعدة البيانات

### 9.3 Clean Architecture (الهندسة النظيفة)

```
الهندسة النظيفة
├── Domain Layer (طبقة النطاق)
│   └── الكيانات والمنطق الأساسي
├── Application Layer (طبقة التطبيق)
│   └── حالات الاستخدام والمعالجات
├── Infrastructure Layer (طبقة البنية التحتية)
│   └── الوصول للبيانات والخدمات الخارجية
└── API Layer (طبقة الواجهة)
    └── نقاط النهاية والمتحكمات
```

### 9.4 Domain-Driven Design (التصميم المبني على النطاق)

| المفهوم | الوصف |
|---------|-------|
| **Aggregate Root** | جذر التجميع |
| **Entity** | الكيان |
| **Value Object** | كائن القيمة |
| **Domain Event** | حدث النطاق |

### 9.5 Database per Service (قاعدة بيانات لكل خدمة)

**الفوائد:**
- استقلالية الخدمات
- حرية اختيار التقنية المناسبة
- سهولة التوسع

### 9.6 Event Sourcing (إعادة بناء الحالة من الأحداث)

- مُطبَّق في خدمة الكتالوج باستخدام **Marten**
- تخزين جميع التغييرات كأحداث
- إمكانية الرجوع لأي نقطة زمنية

---

## 10. التطويرات المستقبلية

### 10.1 قيد التطوير 🚧

| الميزة | الوصف | الأولوية |
|--------|-------|---------|
| إكمال Ordering | تفعيل نقاط النهاية | عالية |
| الاختبارات | إضافة Unit و Integration Tests | عالية |
| Async Messaging | RabbitMQ للرسائل غير المتزامنة | عالية |

### 10.2 مخطط للمستقبل 📋

| الميزة | الوصف | الأولوية |
|--------|-------|---------|
| **API Gateway** | بوابة موحدة (Ocelot/YARP) | متوسطة |
| **Service Discovery** | اكتشاف الخدمات (Consul) | متوسطة |
| **CI/CD** | GitHub Actions | متوسطة |
| **Observability** | Serilog + OpenTelemetry | متوسطة |
| **Health Checks** | فحوصات الصحة | منخفضة |
| **Rate Limiting** | تحديد المعدل | منخفضة |

### 10.3 التحسينات المقترحة

1. **المرونة والتحمل**
   - تطبيق Circuit Breaker باستخدام Polly
   - سياسات إعادة المحاولة
   - عزل الأخطاء

2. **المراقبة**
   - Prometheus + Grafana للمقاييس
   - Jaeger للتتبع الموزع
   - ELK Stack للسجلات

3. **الأمان**
   - مصادقة JWT
   - تحديد المعدل
   - HTTPS إلزامي

---

## 📞 معلومات الاتصال

**المطور:** حازم الياعري  
**GitHub:** [@AlyaariHazem](https://github.com/AlyaariHazem)  
**المشروع:** [Microservices Repository](https://github.com/AlyaariHazem/Microservices)

---

## 📄 الترخيص

هذا المشروع مرخص تحت **MIT License**

---

<div align="center">

**⭐ إذا أعجبك المشروع، لا تنسَ إضافة نجمة على GitHub!**

*تم إعداد هذا التوثيق في يناير 2026*

</div>
