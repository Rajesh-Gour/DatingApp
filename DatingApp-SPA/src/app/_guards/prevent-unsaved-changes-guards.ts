import { Injectable } from "@angular/core";
import { CanDeactivate } from '@angular/router';
import { MemberEditComponent } from '../_members/member-edit/member-edit.component';

@Injectable({
    providedIn: 'root'
})

export class PreventUnsavedChanges implements CanDeactivate<MemberEditComponent> {

    canDeactivate(component: MemberEditComponent) {

        if (component.editForm.dirty) {
            
            return confirm('Are you sure you wanted to continue ? Any unsaved changes will be lost');
        }

        return true;
    }
}