## Global Variables
كل بيانات الـCustomers والـProducts والـOrders متخزنة كـglobal variables، فأي function تقدر تعدل فيها مباشرة.


## No Classes / Objects
مفيش Customer أو Product أو Order كـobject حقيقي. بيانات كل كيان متفرقة في arrays.



## Parallel Arrays
بيانات العميل مثلًا موزعة بين: customerIds, customerNames, customerEmails, customerCities, customerIsVip.
وده يخلي الحفاظ على إن كل البيانات تخص نفس العميل أصعب.
Orders تعتمد على Indexes
الـOrder بيشير للـCustomer والـProduct عن طريق indexes في arrays بدل ما يكون عنده objects واضحة.


## Fixed-size arrays
البرنامج محدود بـ50 customer و50 product و100 order و20 line لكل order.


## Weak Encapsulation
مفيش حماية حقيقية للبيانات؛ الـfunctions بتوصل مباشرة للـarrays والـcounters وتغيرهم.


## Responsibilities scattered
منطق الـCustomer والـProduct والـOrder والحساب والدفع والطباعة كله متوزع في functions