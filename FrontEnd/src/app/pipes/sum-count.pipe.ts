import { Pipe, PipeTransform } from '@angular/core';
import { StatusBreakdown } from '../services/dashboard.service';

@Pipe({ name: 'sumCount', standalone: false })
export class SumCountPipe implements PipeTransform {
    transform(items: StatusBreakdown[]): number {
        return (items || []).reduce((s, i) => s + i.count, 0);
    }
}
