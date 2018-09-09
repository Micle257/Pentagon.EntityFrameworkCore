namespace Pentagon.EntityFrameworkCore.Abstractions.Entities {
    using System.ComponentModel.DataAnnotations.Schema;

    public interface IDeletedUserEntitySupport
    {
        /// <summary> Gets or sets the user's name that deleted this entity (row). </summary>
        /// <value> The <see cref="string" />. </value>
        [Column(Order = OrderConstants.DeletedUser)]
        string DeletedUser { get; set; }
    }
}