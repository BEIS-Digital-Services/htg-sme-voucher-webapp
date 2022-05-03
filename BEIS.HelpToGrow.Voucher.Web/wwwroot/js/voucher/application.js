(function () {
    document.body.className = ((document.body.className) ? document.body.className + ' js-enabled' : 'js-enabled');
})();

initValidation = () => {
    const validator = new MOJFrontend.FormValidator(document.forms[0]);

    validator.addValidator('product_type', [{
        method: function (field) {
            if (field !== undefined) {
                return field.value.trim().length > 0;
            }
        }
    }]);

    validator.addValidator('product_id', [{
        method: function (field) {
            if(field !== undefined){
                return field.value.trim().length > 0;
            }
        }
    }]);
    
    validator.addValidator('firstTime', [{
        method: function (field) {
            if(field !== undefined){
                return field.value.trim().length > 0;
            }
        }
    }]);

    validator.addValidator('majorupgrade', [{
        method: function (field) {
            if(field !== undefined){
                return field.value.trim().length > 0;
            }
        }
    }]);

    validator.addValidator('addons', [{
        method: function (field) {
            if (field !== undefined) {
                return field.value.trim().length > 0;
            }
        }
    }]);

    validator.addValidator('companySize', [{
        method: function (field) {
            if (field !== undefined) {
                return field.value.trim().length > 0;
            }
        }
    }]);

    validator.addValidator('hasCompanyNumber', [{
        method: function (field) {
            if (field !== undefined) {
                return field.value.trim().length > 0;
            }
        }
    }]);
    
    validator.addValidator('companies-house-number', [{
        method: function (field) {
            if (field !== undefined) {
                return field.value.trim().length > 0;
            }
        }
    }]);

    validator.addValidator('hasFCANumber', [{
        method: function (field) {
            if (field !== undefined) {
                return field.value.trim().length > 0;
            }
        }
    }]);

    validator.addValidator('fcaNumber', [{
        method: function (field) {
            if (field !== undefined) {
                return field.value.trim().length > 0;
            }
        }
    }]);
}