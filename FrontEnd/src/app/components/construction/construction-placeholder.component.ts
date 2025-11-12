import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
    selector: 'app-construction-placeholder',
    standalone: true,
    imports: [CommonModule],
    template: `
        <div class="content-wrapper">
            <section class="content-header">
                <div class="container-fluid">
                    <div class="row mb-2">
                        <div class="col-sm-6">
                            <h1>{{ title }}</h1>
                        </div>
                    </div>
                </div>
            </section>
            <section class="content">
                <div class="container-fluid">
                    <div class="card">
                        <div class="card-body">
                            <p class="text-center p-5">
                                <i class="fas fa-tools fa-3x text-muted mb-3"></i>
                                <br />
                                <strong>{{ title }} - Coming Soon</strong>
                                <br />
                                <span class="text-muted">This feature is currently under development.</span>
                            </p>
                        </div>
                    </div>
                </div>
            </section>
        </div>
    `,
})
export class ConstructionPlaceholderComponent {
    title: string = 'Construction Feature';
}
