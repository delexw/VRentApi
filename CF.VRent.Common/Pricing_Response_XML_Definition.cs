[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.17929")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
public partial class Price
{

    private PriceRental rentalField;

    private PriceInsuranceFee insuranceFeeField;

    private PriceFuel fuelField;

    private PriceFine fineField;

    private double totalField;

    private bool totalFieldSpecified;

    private string idField;

    private string timestampField;

    private string preauthField;

    /// <remarks/>
    public PriceRental Rental
    {
        get
        {
            return this.rentalField;
        }
        set
        {
            this.rentalField = value;
        }
    }

    /// <remarks/>
    public PriceInsuranceFee InsuranceFee
    {
        get
        {
            return this.insuranceFeeField;
        }
        set
        {
            this.insuranceFeeField = value;
        }
    }

    /// <remarks/>
    public PriceFuel Fuel
    {
        get
        {
            return this.fuelField;
        }
        set
        {
            this.fuelField = value;
        }
    }

    /// <remarks/>
    public PriceFine Fine
    {
        get
        {
            return this.fineField;
        }
        set
        {
            this.fineField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public double total
    {
        get
        {
            return this.totalField;
        }
        set
        {
            this.totalField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool totalSpecified
    {
        get
        {
            return this.totalFieldSpecified;
        }
        set
        {
            this.totalFieldSpecified = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string id
    {
        get
        {
            return this.idField;
        }
        set
        {
            this.idField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string timestamp
    {
        get
        {
            return this.timestampField;
        }
        set
        {
            this.timestampField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute("pre-auth")]
    public string preauth
    {
        get
        {
            return this.preauthField;
        }
        set
        {
            this.preauthField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.17929")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class PriceRental
{

    private PriceRentalItem[] itemField;

    private double totalField;

    private bool totalFieldSpecified;

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("item")]
    public PriceRentalItem[] item
    {
        get
        {
            return this.itemField;
        }
        set
        {
            this.itemField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public double total
    {
        get
        {
            return this.totalField;
        }
        set
        {
            this.totalField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool totalSpecified
    {
        get
        {
            return this.totalFieldSpecified;
        }
        set
        {
            this.totalFieldSpecified = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.17929")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class PriceRentalItem
{

    private PriceRentalItemPeriod[] periodField;

    private string[] textField;

    private string typeField;

    private double totalField;

    private bool totalFieldSpecified;

    private string fromField;

    private string toField;

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("period")]
    public PriceRentalItemPeriod[] period
    {
        get
        {
            return this.periodField;
        }
        set
        {
            this.periodField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTextAttribute()]
    public string[] Text
    {
        get
        {
            return this.textField;
        }
        set
        {
            this.textField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string type
    {
        get
        {
            return this.typeField;
        }
        set
        {
            this.typeField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public double total
    {
        get
        {
            return this.totalField;
        }
        set
        {
            this.totalField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool totalSpecified
    {
        get
        {
            return this.totalFieldSpecified;
        }
        set
        {
            this.totalFieldSpecified = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string from
    {
        get
        {
            return this.fromField;
        }
        set
        {
            this.fromField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string to
    {
        get
        {
            return this.toField;
        }
        set
        {
            this.toField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.17929")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class PriceRentalItemPeriod
{

    private string fromField;

    private string toField;

    private string valueField;

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string from
    {
        get
        {
            return this.fromField;
        }
        set
        {
            this.fromField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string to
    {
        get
        {
            return this.toField;
        }
        set
        {
            this.toField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTextAttribute()]
    public string Value
    {
        get
        {
            return this.valueField;
        }
        set
        {
            this.valueField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.17929")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class PriceInsuranceFee
{

    private double totalField;

    private bool totalFieldSpecified;

    private string valueField;

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public double total
    {
        get
        {
            return this.totalField;
        }
        set
        {
            this.totalField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool totalSpecified
    {
        get
        {
            return this.totalFieldSpecified;
        }
        set
        {
            this.totalFieldSpecified = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTextAttribute()]
    public string Value
    {
        get
        {
            return this.valueField;
        }
        set
        {
            this.valueField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.17929")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class PriceFuel
{

    private sbyte kilometerField;

    private bool kilometerFieldSpecified;

    private double totalField;

    private bool totalFieldSpecified;

    private string valueField;

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public sbyte kilometer
    {
        get
        {
            return this.kilometerField;
        }
        set
        {
            this.kilometerField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool kilometerSpecified
    {
        get
        {
            return this.kilometerFieldSpecified;
        }
        set
        {
            this.kilometerFieldSpecified = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public double total
    {
        get
        {
            return this.totalField;
        }
        set
        {
            this.totalField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool totalSpecified
    {
        get
        {
            return this.totalFieldSpecified;
        }
        set
        {
            this.totalFieldSpecified = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTextAttribute()]
    public string Value
    {
        get
        {
            return this.valueField;
        }
        set
        {
            this.valueField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.17929")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class PriceFine
{

    private PriceFineItem[] itemField;

    private double totalField;

    private bool totalFieldSpecified;

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("item")]
    public PriceFineItem[] item
    {
        get
        {
            return this.itemField;
        }
        set
        {
            this.itemField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public double total
    {
        get
        {
            return this.totalField;
        }
        set
        {
            this.totalField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool totalSpecified
    {
        get
        {
            return this.totalFieldSpecified;
        }
        set
        {
            this.totalFieldSpecified = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.17929")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class PriceFineItem
{

    private string typeField;

    private string descriptionField;

    private double totalField;

    private bool totalFieldSpecified;

    private string valueField;

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string type
    {
        get
        {
            return this.typeField;
        }
        set
        {
            this.typeField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string description
    {
        get
        {
            return this.descriptionField;
        }
        set
        {
            this.descriptionField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public double total
    {
        get
        {
            return this.totalField;
        }
        set
        {
            this.totalField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool totalSpecified
    {
        get
        {
            return this.totalFieldSpecified;
        }
        set
        {
            this.totalFieldSpecified = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTextAttribute()]
    public string Value
    {
        get
        {
            return this.valueField;
        }
        set
        {
            this.valueField = value;
        }
    }
}