namespace Pentagon.EntityFrameworkCore.Abstractions.Entities {
    using System.ComponentModel.DataAnnotations.Schema;

    public interface IUpdatedUserEntitySupport
    {
        /// <summary> Gets or sets the user's name that updated this entity (row). </summary>
        /// <value> The <see cref="string" />. </value>
        [Column(Order = OrderConstants.UpdatedUser)]
        string UpdatedUser { get; set; }
    }
}