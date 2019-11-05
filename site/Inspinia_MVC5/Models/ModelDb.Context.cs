﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Inspinia_MVC5.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class ModelDb : DbContext
    {
        public ModelDb()
            : base("name=ModelDb")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Card> Cards { get; set; }
        public virtual DbSet<CardStatus> CardStatuses { get; set; }
        public virtual DbSet<CardType> CardTypes { get; set; }
        public virtual DbSet<Company> Companies { get; set; }
        public virtual DbSet<CountersOperating> CountersOperatings { get; set; }
        public virtual DbSet<CountersTotal> CountersTotals { get; set; }
        public virtual DbSet<Event> Events { get; set; }
        public virtual DbSet<EventCash> EventCashes { get; set; }
        public virtual DbSet<EventKind> EventKinds { get; set; }
        public virtual DbSet<EventMode> EventModes { get; set; }
        public virtual DbSet<EventSimple> EventSimples { get; set; }
        public virtual DbSet<Mode> Modes { get; set; }
        public virtual DbSet<Operation> Operations { get; set; }
        public virtual DbSet<OperationType> OperationTypes { get; set; }
        public virtual DbSet<Owner> Owners { get; set; }
        public virtual DbSet<Post> Posts { get; set; }
        public virtual DbSet<Psce> Psces { get; set; }
        public virtual DbSet<Region> Regions { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Wash> Washes { get; set; }
        public virtual DbSet<Device> Devices { get; set; }
        public virtual DbSet<Finance> Finances { get; set; }
        public virtual DbSet<FinanceType> FinanceTypes { get; set; }
    
        public virtual ObjectResult<GetCardsOperations_Result> GetCardsOperations(string p_Phone, string p_Cardnum, string p_CardTypeCode, string p_CardStatusName, string p_OperationTypeName, Nullable<System.DateTime> p_OperationDateBeg, Nullable<System.DateTime> p_OperationDateEnd, Nullable<int> p_LocalizedBy, Nullable<int> p_LocalizedID)
        {
            var p_PhoneParameter = p_Phone != null ?
                new ObjectParameter("p_Phone", p_Phone) :
                new ObjectParameter("p_Phone", typeof(string));
    
            var p_CardnumParameter = p_Cardnum != null ?
                new ObjectParameter("p_Cardnum", p_Cardnum) :
                new ObjectParameter("p_Cardnum", typeof(string));
    
            var p_CardTypeCodeParameter = p_CardTypeCode != null ?
                new ObjectParameter("p_CardTypeCode", p_CardTypeCode) :
                new ObjectParameter("p_CardTypeCode", typeof(string));
    
            var p_CardStatusNameParameter = p_CardStatusName != null ?
                new ObjectParameter("p_CardStatusName", p_CardStatusName) :
                new ObjectParameter("p_CardStatusName", typeof(string));
    
            var p_OperationTypeNameParameter = p_OperationTypeName != null ?
                new ObjectParameter("p_OperationTypeName", p_OperationTypeName) :
                new ObjectParameter("p_OperationTypeName", typeof(string));
    
            var p_OperationDateBegParameter = p_OperationDateBeg.HasValue ?
                new ObjectParameter("p_OperationDateBeg", p_OperationDateBeg) :
                new ObjectParameter("p_OperationDateBeg", typeof(System.DateTime));
    
            var p_OperationDateEndParameter = p_OperationDateEnd.HasValue ?
                new ObjectParameter("p_OperationDateEnd", p_OperationDateEnd) :
                new ObjectParameter("p_OperationDateEnd", typeof(System.DateTime));
    
            var p_LocalizedByParameter = p_LocalizedBy.HasValue ?
                new ObjectParameter("p_LocalizedBy", p_LocalizedBy) :
                new ObjectParameter("p_LocalizedBy", typeof(int));
    
            var p_LocalizedIDParameter = p_LocalizedID.HasValue ?
                new ObjectParameter("p_LocalizedID", p_LocalizedID) :
                new ObjectParameter("p_LocalizedID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetCardsOperations_Result>("GetCardsOperations", p_PhoneParameter, p_CardnumParameter, p_CardTypeCodeParameter, p_CardStatusNameParameter, p_OperationTypeNameParameter, p_OperationDateBegParameter, p_OperationDateEndParameter, p_LocalizedByParameter, p_LocalizedIDParameter);
        }
    
        public virtual ObjectResult<GetWashAmounts_Result> GetWashAmounts(Nullable<int> p_RegionCode, string p_WashCode, string p_PostCode, Nullable<System.DateTime> p_DateBeg, Nullable<System.DateTime> p_DateEnd)
        {
            var p_RegionCodeParameter = p_RegionCode.HasValue ?
                new ObjectParameter("p_RegionCode", p_RegionCode) :
                new ObjectParameter("p_RegionCode", typeof(int));
    
            var p_WashCodeParameter = p_WashCode != null ?
                new ObjectParameter("p_WashCode", p_WashCode) :
                new ObjectParameter("p_WashCode", typeof(string));
    
            var p_PostCodeParameter = p_PostCode != null ?
                new ObjectParameter("p_PostCode", p_PostCode) :
                new ObjectParameter("p_PostCode", typeof(string));
    
            var p_DateBegParameter = p_DateBeg.HasValue ?
                new ObjectParameter("p_DateBeg", p_DateBeg) :
                new ObjectParameter("p_DateBeg", typeof(System.DateTime));
    
            var p_DateEndParameter = p_DateEnd.HasValue ?
                new ObjectParameter("p_DateEnd", p_DateEnd) :
                new ObjectParameter("p_DateEnd", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetWashAmounts_Result>("GetWashAmounts", p_RegionCodeParameter, p_WashCodeParameter, p_PostCodeParameter, p_DateBegParameter, p_DateEndParameter);
        }
    
        public virtual ObjectResult<GetPostAmounts_Result> GetPostAmounts(Nullable<int> p_RegionCode, string p_WashCode, string p_PostCode, Nullable<System.DateTime> p_DateBeg, Nullable<System.DateTime> p_DateEnd)
        {
            var p_RegionCodeParameter = p_RegionCode.HasValue ?
                new ObjectParameter("p_RegionCode", p_RegionCode) :
                new ObjectParameter("p_RegionCode", typeof(int));
    
            var p_WashCodeParameter = p_WashCode != null ?
                new ObjectParameter("p_WashCode", p_WashCode) :
                new ObjectParameter("p_WashCode", typeof(string));
    
            var p_PostCodeParameter = p_PostCode != null ?
                new ObjectParameter("p_PostCode", p_PostCode) :
                new ObjectParameter("p_PostCode", typeof(string));
    
            var p_DateBegParameter = p_DateBeg.HasValue ?
                new ObjectParameter("p_DateBeg", p_DateBeg) :
                new ObjectParameter("p_DateBeg", typeof(System.DateTime));
    
            var p_DateEndParameter = p_DateEnd.HasValue ?
                new ObjectParameter("p_DateEnd", p_DateEnd) :
                new ObjectParameter("p_DateEnd", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetPostAmounts_Result>("GetPostAmounts", p_RegionCodeParameter, p_WashCodeParameter, p_PostCodeParameter, p_DateBegParameter, p_DateEndParameter);
        }
    
        public virtual ObjectResult<GetCardList_Result> GetCardList(string p_Phone, string p_Cardnum, string p_CardTypeCode, string p_CardStatusName, Nullable<int> p_BalanceMin, Nullable<int> p_BalanceMax, Nullable<System.DateTime> p_ActivationDateBeg, Nullable<System.DateTime> p_ActivationDateEnd, Nullable<int> p_ActivationBy, Nullable<System.DateTime> p_LastOperationDateBeg, Nullable<System.DateTime> p_LastOperationDateEnd, Nullable<int> p_LastOperationBy, Nullable<int> p_IncreaseSumMin, Nullable<int> p_IncreaseSumMax, Nullable<int> p_DecreaseSumMin, Nullable<int> p_DecreaseSumMax, Nullable<int> p_CountOperationMin, Nullable<int> p_CountOperationMax)
        {
            var p_PhoneParameter = p_Phone != null ?
                new ObjectParameter("p_Phone", p_Phone) :
                new ObjectParameter("p_Phone", typeof(string));
    
            var p_CardnumParameter = p_Cardnum != null ?
                new ObjectParameter("p_Cardnum", p_Cardnum) :
                new ObjectParameter("p_Cardnum", typeof(string));
    
            var p_CardTypeCodeParameter = p_CardTypeCode != null ?
                new ObjectParameter("p_CardTypeCode", p_CardTypeCode) :
                new ObjectParameter("p_CardTypeCode", typeof(string));
    
            var p_CardStatusNameParameter = p_CardStatusName != null ?
                new ObjectParameter("p_CardStatusName", p_CardStatusName) :
                new ObjectParameter("p_CardStatusName", typeof(string));
    
            var p_BalanceMinParameter = p_BalanceMin.HasValue ?
                new ObjectParameter("p_BalanceMin", p_BalanceMin) :
                new ObjectParameter("p_BalanceMin", typeof(int));
    
            var p_BalanceMaxParameter = p_BalanceMax.HasValue ?
                new ObjectParameter("p_BalanceMax", p_BalanceMax) :
                new ObjectParameter("p_BalanceMax", typeof(int));
    
            var p_ActivationDateBegParameter = p_ActivationDateBeg.HasValue ?
                new ObjectParameter("p_ActivationDateBeg", p_ActivationDateBeg) :
                new ObjectParameter("p_ActivationDateBeg", typeof(System.DateTime));
    
            var p_ActivationDateEndParameter = p_ActivationDateEnd.HasValue ?
                new ObjectParameter("p_ActivationDateEnd", p_ActivationDateEnd) :
                new ObjectParameter("p_ActivationDateEnd", typeof(System.DateTime));
    
            var p_ActivationByParameter = p_ActivationBy.HasValue ?
                new ObjectParameter("p_ActivationBy", p_ActivationBy) :
                new ObjectParameter("p_ActivationBy", typeof(int));
    
            var p_LastOperationDateBegParameter = p_LastOperationDateBeg.HasValue ?
                new ObjectParameter("p_LastOperationDateBeg", p_LastOperationDateBeg) :
                new ObjectParameter("p_LastOperationDateBeg", typeof(System.DateTime));
    
            var p_LastOperationDateEndParameter = p_LastOperationDateEnd.HasValue ?
                new ObjectParameter("p_LastOperationDateEnd", p_LastOperationDateEnd) :
                new ObjectParameter("p_LastOperationDateEnd", typeof(System.DateTime));
    
            var p_LastOperationByParameter = p_LastOperationBy.HasValue ?
                new ObjectParameter("p_LastOperationBy", p_LastOperationBy) :
                new ObjectParameter("p_LastOperationBy", typeof(int));
    
            var p_IncreaseSumMinParameter = p_IncreaseSumMin.HasValue ?
                new ObjectParameter("p_IncreaseSumMin", p_IncreaseSumMin) :
                new ObjectParameter("p_IncreaseSumMin", typeof(int));
    
            var p_IncreaseSumMaxParameter = p_IncreaseSumMax.HasValue ?
                new ObjectParameter("p_IncreaseSumMax", p_IncreaseSumMax) :
                new ObjectParameter("p_IncreaseSumMax", typeof(int));
    
            var p_DecreaseSumMinParameter = p_DecreaseSumMin.HasValue ?
                new ObjectParameter("p_DecreaseSumMin", p_DecreaseSumMin) :
                new ObjectParameter("p_DecreaseSumMin", typeof(int));
    
            var p_DecreaseSumMaxParameter = p_DecreaseSumMax.HasValue ?
                new ObjectParameter("p_DecreaseSumMax", p_DecreaseSumMax) :
                new ObjectParameter("p_DecreaseSumMax", typeof(int));
    
            var p_CountOperationMinParameter = p_CountOperationMin.HasValue ?
                new ObjectParameter("p_CountOperationMin", p_CountOperationMin) :
                new ObjectParameter("p_CountOperationMin", typeof(int));
    
            var p_CountOperationMaxParameter = p_CountOperationMax.HasValue ?
                new ObjectParameter("p_CountOperationMax", p_CountOperationMax) :
                new ObjectParameter("p_CountOperationMax", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetCardList_Result>("GetCardList", p_PhoneParameter, p_CardnumParameter, p_CardTypeCodeParameter, p_CardStatusNameParameter, p_BalanceMinParameter, p_BalanceMaxParameter, p_ActivationDateBegParameter, p_ActivationDateEndParameter, p_ActivationByParameter, p_LastOperationDateBegParameter, p_LastOperationDateEndParameter, p_LastOperationByParameter, p_IncreaseSumMinParameter, p_IncreaseSumMaxParameter, p_DecreaseSumMinParameter, p_DecreaseSumMaxParameter, p_CountOperationMinParameter, p_CountOperationMaxParameter);
        }
    
        public virtual ObjectResult<GetCardListMinMaxDiapasons_Result> GetCardListMinMaxDiapasons(string p_Phone, string p_Cardnum, string p_CardTypeCode, string p_CardStatusName, Nullable<int> p_BalanceMin, Nullable<int> p_BalanceMax, Nullable<System.DateTime> p_ActivationDateBeg, Nullable<System.DateTime> p_ActivationDateEnd, Nullable<int> p_ActivationBy, Nullable<System.DateTime> p_LastOperationDateBeg, Nullable<System.DateTime> p_LastOperationDateEnd, Nullable<int> p_LastOperationBy, Nullable<int> p_IncreaseSumMin, Nullable<int> p_IncreaseSumMax, Nullable<int> p_DecreaseSumMin, Nullable<int> p_DEcreaseSumMax, Nullable<int> p_CountOperationMin, Nullable<int> p_CountOperationMax)
        {
            var p_PhoneParameter = p_Phone != null ?
                new ObjectParameter("p_Phone", p_Phone) :
                new ObjectParameter("p_Phone", typeof(string));
    
            var p_CardnumParameter = p_Cardnum != null ?
                new ObjectParameter("p_Cardnum", p_Cardnum) :
                new ObjectParameter("p_Cardnum", typeof(string));
    
            var p_CardTypeCodeParameter = p_CardTypeCode != null ?
                new ObjectParameter("p_CardTypeCode", p_CardTypeCode) :
                new ObjectParameter("p_CardTypeCode", typeof(string));
    
            var p_CardStatusNameParameter = p_CardStatusName != null ?
                new ObjectParameter("p_CardStatusName", p_CardStatusName) :
                new ObjectParameter("p_CardStatusName", typeof(string));
    
            var p_BalanceMinParameter = p_BalanceMin.HasValue ?
                new ObjectParameter("p_BalanceMin", p_BalanceMin) :
                new ObjectParameter("p_BalanceMin", typeof(int));
    
            var p_BalanceMaxParameter = p_BalanceMax.HasValue ?
                new ObjectParameter("p_BalanceMax", p_BalanceMax) :
                new ObjectParameter("p_BalanceMax", typeof(int));
    
            var p_ActivationDateBegParameter = p_ActivationDateBeg.HasValue ?
                new ObjectParameter("p_ActivationDateBeg", p_ActivationDateBeg) :
                new ObjectParameter("p_ActivationDateBeg", typeof(System.DateTime));
    
            var p_ActivationDateEndParameter = p_ActivationDateEnd.HasValue ?
                new ObjectParameter("p_ActivationDateEnd", p_ActivationDateEnd) :
                new ObjectParameter("p_ActivationDateEnd", typeof(System.DateTime));
    
            var p_ActivationByParameter = p_ActivationBy.HasValue ?
                new ObjectParameter("p_ActivationBy", p_ActivationBy) :
                new ObjectParameter("p_ActivationBy", typeof(int));
    
            var p_LastOperationDateBegParameter = p_LastOperationDateBeg.HasValue ?
                new ObjectParameter("p_LastOperationDateBeg", p_LastOperationDateBeg) :
                new ObjectParameter("p_LastOperationDateBeg", typeof(System.DateTime));
    
            var p_LastOperationDateEndParameter = p_LastOperationDateEnd.HasValue ?
                new ObjectParameter("p_LastOperationDateEnd", p_LastOperationDateEnd) :
                new ObjectParameter("p_LastOperationDateEnd", typeof(System.DateTime));
    
            var p_LastOperationByParameter = p_LastOperationBy.HasValue ?
                new ObjectParameter("p_LastOperationBy", p_LastOperationBy) :
                new ObjectParameter("p_LastOperationBy", typeof(int));
    
            var p_IncreaseSumMinParameter = p_IncreaseSumMin.HasValue ?
                new ObjectParameter("p_IncreaseSumMin", p_IncreaseSumMin) :
                new ObjectParameter("p_IncreaseSumMin", typeof(int));
    
            var p_IncreaseSumMaxParameter = p_IncreaseSumMax.HasValue ?
                new ObjectParameter("p_IncreaseSumMax", p_IncreaseSumMax) :
                new ObjectParameter("p_IncreaseSumMax", typeof(int));
    
            var p_DecreaseSumMinParameter = p_DecreaseSumMin.HasValue ?
                new ObjectParameter("p_DecreaseSumMin", p_DecreaseSumMin) :
                new ObjectParameter("p_DecreaseSumMin", typeof(int));
    
            var p_DEcreaseSumMaxParameter = p_DEcreaseSumMax.HasValue ?
                new ObjectParameter("p_DEcreaseSumMax", p_DEcreaseSumMax) :
                new ObjectParameter("p_DEcreaseSumMax", typeof(int));
    
            var p_CountOperationMinParameter = p_CountOperationMin.HasValue ?
                new ObjectParameter("p_CountOperationMin", p_CountOperationMin) :
                new ObjectParameter("p_CountOperationMin", typeof(int));
    
            var p_CountOperationMaxParameter = p_CountOperationMax.HasValue ?
                new ObjectParameter("p_CountOperationMax", p_CountOperationMax) :
                new ObjectParameter("p_CountOperationMax", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetCardListMinMaxDiapasons_Result>("GetCardListMinMaxDiapasons", p_PhoneParameter, p_CardnumParameter, p_CardTypeCodeParameter, p_CardStatusNameParameter, p_BalanceMinParameter, p_BalanceMaxParameter, p_ActivationDateBegParameter, p_ActivationDateEndParameter, p_ActivationByParameter, p_LastOperationDateBegParameter, p_LastOperationDateEndParameter, p_LastOperationByParameter, p_IncreaseSumMinParameter, p_IncreaseSumMaxParameter, p_DecreaseSumMinParameter, p_DEcreaseSumMaxParameter, p_CountOperationMinParameter, p_CountOperationMaxParameter);
        }
    
        public virtual ObjectResult<GetFinanceList_Result> GetFinanceList(Nullable<System.DateTime> p_DateBeg, Nullable<System.DateTime> p_DateEnd)
        {
            var p_DateBegParameter = p_DateBeg.HasValue ?
                new ObjectParameter("p_DateBeg", p_DateBeg) :
                new ObjectParameter("p_DateBeg", typeof(System.DateTime));
    
            var p_DateEndParameter = p_DateEnd.HasValue ?
                new ObjectParameter("p_DateEnd", p_DateEnd) :
                new ObjectParameter("p_DateEnd", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetFinanceList_Result>("GetFinanceList", p_DateBegParameter, p_DateEndParameter);
        }
    }
}
