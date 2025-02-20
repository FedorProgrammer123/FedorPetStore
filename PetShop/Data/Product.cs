//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PetShop.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class Product
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Product()
        {
            this.OrderProduct = new HashSet<OrderProduct>();
        }
    
        public string ProductArticleNumber { get; set; }
        public Nullable<int> IDSupply { get; set; }
        public int IDUnits { get; set; }
        public decimal Cost { get; set; }
        public Nullable<int> MaxDiscount { get; set; }
        public int IDProducer { get; set; }
        public int IDProvider { get; set; }
        public int IDCategoryProduct { get; set; }
        public Nullable<int> CurrentDiscount { get; set; }
        public int CountOnStorage { get; set; }
        public string Description { get; set; }
        public string ProductName { get; set; }
        public byte[] ProductPhoto { get; set; }
    
        public virtual CategoryProduct CategoryProduct { get; set; }
        public virtual NameofSupply NameofSupply { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderProduct> OrderProduct { get; set; }
        public virtual Producer Producer { get; set; }
        public virtual Provider Provider { get; set; }
        public virtual Units Units { get; set; }
    }
}
