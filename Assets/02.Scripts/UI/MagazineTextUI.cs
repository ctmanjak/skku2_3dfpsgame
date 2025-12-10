using TMPro;
using UnityEngine;
using Weapon;

namespace UI
{
    public class MagazineTextUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _magazineText;

        private float _leftAmmoInGun;
        private float _leftAmmoInMagazines;

        public void ChangeValueOnGun(Gun gun)
        {
            _leftAmmoInGun = gun.GetLeftAmmo();
            ChangeValue();
        }

        public void ChangeValueOnMagazines(Gun gun, Magazine[] magazines)
        {
            _leftAmmoInMagazines = 0;
            foreach (var magazine in magazines)
            {
                _leftAmmoInMagazines += magazine.GetLeftAmmo();
            }

            ChangeValueOnGun(gun);
            ChangeValue();
        }

        private void ChangeValue()
        {
            _magazineText.text = $"{_leftAmmoInGun} / {_leftAmmoInMagazines}";
        }
    }
}