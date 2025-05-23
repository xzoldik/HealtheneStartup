﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Dtos.QuestionnaireDtos
{
    public class AddPreferencesPatientDto
    {
        [Required(ErrorMessage = "UserID is required")]
        public required int UserID { get; set; }

        [Required(ErrorMessage = "Role is required")]
        [RegularExpression("^(patient)$", ErrorMessage = "Role must be patient")]
        public required string UserRole { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        [RegularExpression("^(رجل|إمرأة)$", ErrorMessage = "Gender must be رجل or إمرأة")]
        public required string Gender { get; set; }

        [Required(ErrorMessage = "Religion is required")]
        [RegularExpression("^(مسلم|مسيحي|يهودي|أفضل أن لا أقول)$", ErrorMessage = "Religion must be one of: مسلم, مسيحي, يهودي, or أفضل أن لا أقول")]
        public required string Religion { get; set; }

        [Required(ErrorMessage = "TreatingExperience is required")]
        [RegularExpression("^(نعم|لا)$", ErrorMessage = "TreatingExperience must be one of: نعم or لا")]
        public required string TreatingExperience { get; set; }

        [Required(ErrorMessage = "Age is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Age must be a non-negative number")]
        public required int Age { get; set; }

        [Required(ErrorMessage = "MaritalStatus is required")]
        [RegularExpression("^(متزوج|أعزب|مطلق)$", ErrorMessage = "MaritalStatus must be متزوج or مطلق or أعزب")]
        public required string MaritalStatus { get; set; }

        [Required(ErrorMessage = "ReligionImportance is required")]
        [RegularExpression("^(مهم جدا|مهم|مهم إلى حد ما|غير مهم)$", ErrorMessage = "ReligionImportance must be one of: مهم جدا, مهم, مهم إلى حد ما, or غير مهم")]
        public required string ReligionImportance { get; set; }

        [Required(ErrorMessage = "PsychiatryTreatment is required")]
        [RegularExpression("^(نعم|لا)$", ErrorMessage = "PsychiatryTreatment must be one of: نعم or لا")]
        public required string PsychiatryTreatment { get; set; }

        [Required(ErrorMessage = "TreatmentReason is required")]
        [RegularExpression("^(قلق|اكتئاب|ضغوط العمل|مزاجي يؤثر في عملي أو دراستي|أعاني في بناء علاقات جديدة أو الحفاظ على علاقاتي|لم أستطع ايجاد هدف أو معنى لحياتي|أنا حزين|لقد تعرضت لصدمة نفسية|أحتاج للاستشارة و التحدث حول تحدي جديد|أريد أن أكسب الثقة في نفسي|أريد أن أطور نفسي و لا أعرف من أين أبدأ|صديق لي رشح هذا التطبيق|فقط أكتشف|غيرها)$", ErrorMessage = "TreatmentReason is required and must be one of the provided options")]
        public required string TreatmentReason { get; set; }

        [Required(ErrorMessage = "PhysicalHealth is required")]
        [RegularExpression("^(جيدة|متوسطة|سيئة)$", ErrorMessage = "PhysicalHealth must be one of: جيدة, متوسطة, or سيئة")]
        public required string PhysicalHealth { get; set; }

        [Required(ErrorMessage = "FoodHabits is required")]
        [RegularExpression("^(جيدة|متوسطة|سيئة)$", ErrorMessage = "FoodHabits must be one of: جيدة, متوسطة, or سيئة")]
        public required string FoodHabits { get; set; }

        [Required(ErrorMessage = "Depression is required")]
        [RegularExpression("^(نعم|لا)$", ErrorMessage = "Depression must be either نعم or لا")]
        public required string Depression { get; set; }

        [Required(ErrorMessage = "Employment is required")]
        [RegularExpression("^(موظف|عاطل عن العمل)$", ErrorMessage = "Employment must be either موظف or عاطل عن العمل")]
        public required string Employment { get; set; }

        [Required(ErrorMessage = "Intimacy is required")]
        [RegularExpression("^(نعم|لا)$", ErrorMessage = "Intimacy must be either نعم or لا")]
        public required string Intimacy { get; set; }

        [Required(ErrorMessage = "Alcoholic is required")]
        [RegularExpression("^(على الإطلاق|نادرا|شهريات|أسبوعيا|يوميا)$", ErrorMessage = "Alcoholic must be one of: على الإطلاق, نادرا, شهريات, أسبوعيا, or يوميا")]
        public required string Alcoholic { get; set; }

        [Required(ErrorMessage = "Suicide is required")]
        [RegularExpression("^(أبدا|قبل سنة|قبل 3 أشهر|قبل شهرين|قبل أسوعبين|في الأيام القليلة الفائتة)$", ErrorMessage = "Suicide must be one of: أبدا, قبل سنة, قبل 3 أشهر, قبل شهرين, قبل أسوعبين, or في الأيام القليلة الفائتة")]
        public required string Suicide { get; set; }

        [Required(ErrorMessage = "PreferedLanguage is required")]
        [RegularExpression("^(عربية|فرنسية|انجليزية|أمازيغية)$", ErrorMessage = "PreferedLanguage must be one of: عربية, فرنسية, انجليزية, or أمازيغية")]
        public required string PreferedLanguage { get; set; }

        [Required(ErrorMessage = "PreferedGender is required")]
        [RegularExpression("^(رجل|امرأة)$", ErrorMessage = "PreferedGender must be either رجل or امرأة")]
        public required string PreferedGender { get; set; }

        [Required(ErrorMessage = "ExpectationTherapist is required")]
        [RegularExpression("^(يستمع لك|يكتشف ماضيك|يعلمك مهارة جديدة|يرشدني إلى تحقيق أهدافي|يتحقق من صحتي كل مرة)$", ErrorMessage = "ExpectationTherapist must be one of the provided options")]
        public required string ExpectationTherapist { get; set; }

        [Required(ErrorMessage = "DirectOrGentel is required")]
        [RegularExpression("^(لطيف|لطيف إلى حد ما|لا تفضيل|مباشر إلى حد ما|مباشر)$", ErrorMessage = "DirectOrGentel must be one of: لطيف, لطيف إلى حد ما, لا تفضيل, مباشر إلى حد ما, or مباشر")]
        public required string DirectOrGentel { get; set; }

        [Required(ErrorMessage = "StructuredOrFlexible is required")]
        [RegularExpression("^(مرن|مرن إلى حد ما|لا تفضيل|منظم إلى حد ما|منظم)$", ErrorMessage = "StructuredOrFlexible must be one of: مرن, مرن إلى حد ما, لا تفضيل, منظم إلى حد ما, or منظم")]
        public required string StructuredOrFlexible { get; set; }

        [Required(ErrorMessage = "OfficialOrCasual is required")]
        [RegularExpression("^(معتاد|معتاد إلى حد ما|بدون تفضيل|رسمي إلى حد ما|رسمي)$", ErrorMessage = "OfficialOrCasual must be one of: معتاد, معتاد إلى حد ما, بدون تفضيل, رسمي إلى حد ما, or رسمي")]
        public required string OfficialOrCasual { get; set; }
    }
}