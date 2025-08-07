import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CrmFormService } from '../../services/crm/crm-form.service';
import { CrmFormTemplate, CreateCrmFormTemplateDto } from '../../models/crm/crm.model';
import { NotificationService } from '../../services/notification.service';
import { BusinessNatureService } from '../../services/business-nature.service';

@Component({
    selector: 'app-crm-form',
    templateUrl: './crm-form.component.html',
    styleUrls: ['./crm-form.component.css'],
    standalone: false,
})
export class CrmFormComponent implements OnInit {
    templateForm: FormGroup;
    templates: CrmFormTemplate[] = [];
    businessNatures: any[] = [];
    editingTemplate: CrmFormTemplate | null = null;
    loading = false;
    selectedTemplate: CrmFormTemplate | null = null;

    // Sample JSON Schema for form templates
    sampleJsonSchema = JSON.stringify({
        "$schema": "http://json-schema.org/draft-07/schema#",
        "type": "object",
        "properties": {
            "customerName": {
                "type": "string",
                "title": "Customer Name",
                "maxLength": 100
            },
            "projectType": {
                "type": "string",
                "title": "Project Type",
                "enum": ["Website", "Mobile App", "SEO", "Social Media"]
            },
            "budget": {
                "type": "number",
                "title": "Budget ($)",
                "minimum": 0
            },
            "timeline": {
                "type": "string",
                "title": "Timeline",
                "enum": ["1-2 weeks", "1 month", "2-3 months", "3+ months"]
            },
            "requirements": {
                "type": "string",
                "title": "Requirements",
                "format": "textarea"
            }
        },
        "required": ["customerName", "projectType"]
    }, null, 2);

    constructor(
        private fb: FormBuilder,
        private crmFormService: CrmFormService,
        private businessNatureService: BusinessNatureService,
        private notificationService: NotificationService
    ) {
        this.initializeForm();
    }

    ngOnInit(): void {
        this.loadTemplates();
        this.loadBusinessNatures();
    }

    private initializeForm(): void {
        this.templateForm = this.fb.group({
            name: ['', [Validators.required, Validators.maxLength(200)]],
            description: ['', [Validators.maxLength(1000)]],
            jsonSchema: [this.sampleJsonSchema, [Validators.required]],
            businessNatureId: ['', [Validators.required]],
            isActive: [true],
        });
    }

    private loadTemplates(): void {
        this.loading = true;
        this.crmFormService.getFormTemplates(0, 100).subscribe({
            next: (response) => {
                this.templates = response.result || [];
                this.loading = false;
            },
            error: (error) => {
                console.error('Error loading templates:', error);
                this.notificationService.showError('Failed to load form templates');
                this.loading = false;
            }
        });
    }

    private loadBusinessNatures(): void {
        this.businessNatureService.getBusinessNatures().subscribe({
            next: (businessNatures) => {
                this.businessNatures = businessNatures;
            },
            error: (error) => {
                console.error('Error loading business natures:', error);
            }
        });
    }

    onSubmit(): void {
        if (this.templateForm.valid) {
            this.loading = true;
            const formValue = this.templateForm.value;

            // Validate JSON Schema
            try {
                JSON.parse(formValue.jsonSchema);
            } catch (e) {
                this.notificationService.showError('Invalid JSON Schema format');
                this.loading = false;
                return;
            }

            if (this.editingTemplate) {
                // Update existing template
                this.crmFormService
                    .updateFormTemplate(this.editingTemplate.id, {
                        ...formValue,
                        id: this.editingTemplate.id,
                    })
                    .subscribe({
                        next: () => {
                            this.notificationService.showSuccess('Form template updated successfully');
                            this.loadTemplates();
                            this.resetForm();
                        },
                        error: (error) => {
                            console.error('Error updating template:', error);
                            this.notificationService.showError('Failed to update form template');
                            this.loading = false;
                        }
                    });
            } else {
                // Create new template
                const createTemplateDto: CreateCrmFormTemplateDto = formValue;
                this.crmFormService.createFormTemplate(createTemplateDto).subscribe({
                    next: () => {
                        this.notificationService.showSuccess('Form template created successfully');
                        this.loadTemplates();
                        this.resetForm();
                    },
                    error: (error) => {
                        console.error('Error creating template:', error);
                        this.notificationService.showError('Failed to create form template');
                        this.loading = false;
                    }
                });
            }
        }
    }

    editTemplate(template: CrmFormTemplate): void {
        this.editingTemplate = template;
        this.templateForm.patchValue({
            name: template.name,
            description: template.description,
            jsonSchema: template.jsonSchema,
            businessNatureId: template.businessNatureId,
            isActive: template.isActive,
        });
    }

    deleteTemplate(id: number): void {
        if (confirm('Are you sure you want to delete this form template?')) {
            this.crmFormService.deleteFormTemplate(id).subscribe({
                next: () => {
                    this.notificationService.showSuccess('Form template deleted successfully');
                    this.loadTemplates();
                },
                error: (error) => {
                    console.error('Error deleting template:', error);
                    this.notificationService.showError('Failed to delete form template');
                }
            });
        }
    }

    viewTemplate(template: CrmFormTemplate): void {
        this.selectedTemplate = template;
    }

    closeTemplateView(): void {
        this.selectedTemplate = null;
    }

    resetForm(): void {
        this.editingTemplate = null;
        this.templateForm.reset({
            jsonSchema: this.sampleJsonSchema,
            isActive: true
        });
        this.loading = false;
    }

    formatJsonSchema(jsonSchema: string): string {
        try {
            const parsed = JSON.parse(jsonSchema);
            return JSON.stringify(parsed, null, 2);
        } catch (e) {
            return jsonSchema;
        }
    }

    useSampleSchema(): void {
        this.templateForm.patchValue({
            jsonSchema: this.sampleJsonSchema
        });
    }
}