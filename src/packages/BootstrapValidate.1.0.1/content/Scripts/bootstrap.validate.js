$(function () {
    var elements = {
        fieldValidationSpanAll: 'span.field-validation-valid, span.field-validation-error',
        fieldValidationSpanError: 'span.field-validation-error',
        controlGroupDiv: 'div.control-group',
        form: 'form',
        formInput: 'form input'
    },
    classes = {
        inlineHelp: 'help-inline',
        error: 'error'
    },
    addInlineHelp = function () {
        $(this).addClass(classes.inlineHelp);
    },
    onFormSubmit = function () {
        if ($(this).valid()) {
            $(this).find(elements.controlGroupDiv).each(function () {
                if ($(this).find(elements.fieldValidationSpanError).length == 0) {
                    $(this).removeClass(classes.error);
                }
            });
        }
        else {
            $(this).find(elements.controlGroupDiv).each(function () {
                if ($(this).find(elements.fieldValidationSpanError).length > 0) {
                    $(this).addClass(classes.error);
                }
            });
        }
    },
    onFormInputChange = function () {
        var parentForm = $(this).parents(elements.form);
        if ($(this).valid()) {
            parentForm.find(elements.controlGroupDiv).each(function () {
                if ($(this).find(elements.fieldValidationSpanError).length == 0) {
                    $(this).removeClass(classes.error);
                }
            });
        }
        else {
            parentForm.find(elements.controlGroupDiv).each(function () {
                if ($(this).find(elements.fieldValidationSpanError).length > 0) {
                    $(this).addClass(classes.error);
                }
            });
        }
    },
    forEachForm = function () {
        $(this).find(elements.controlGroupDiv).each(function () {
            if ($(this).find(elements.fieldValidationSpanError).length > 0) {
                $(this).addClass(classes.error);
            }
        });
    };

    $(elements.fieldValidationSpanAll).each(addInlineHelp);

    $(elements.form).submit(onFormSubmit);

    $(elements.formInput).change(onFormInputChange);

    $(elements.form).each(forEachForm);
});